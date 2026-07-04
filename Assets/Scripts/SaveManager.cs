using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using Steamworks;

public static class SaveManager
{
    private const string FileName = "save.dat";
    private static readonly string LocalPath = Path.Combine(Application.persistentDataPath, FileName);
    private static readonly string BackupPath = Path.Combine(Application.persistentDataPath, "save.bak");

    // 키 버전 태그를 포함하면 향후 키 정책 변경 시 복구 용이
    private const string KeySuffix = "_TheSeven_Key_v1";

    // 키 생성: SteamID 기반(로그인 되어 있으면) 또는 디바이스 기반 폴백
    private static byte[] GetKey()
    {
        string baseKey;
        try {
            if (SteamManager.Initialized) {
                baseKey = SteamUser.GetSteamID().ToString() + KeySuffix;
            }
            else {
                baseKey = SystemInfo.deviceUniqueIdentifier + KeySuffix;
            }
        }
        catch {
            baseKey = SystemInfo.deviceUniqueIdentifier + KeySuffix;
        }

        return SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(baseKey)); // 32바이트
    }

    // 외부에서 호출: 저장
    public static void Save(SaveData data)
    {
        try {
            string json = JsonUtility.ToJson(data);
            byte[] encrypted = Encrypt(json);

            // 로컬 저장
            File.WriteAllBytes(LocalPath, encrypted);

            // 백업 생성
            File.Copy(LocalPath, BackupPath, true);

            GameManager.instance.saveCheckPanel.SetActive(true);

            // Steam 클라우드 업로드 (있으면)
            try {
                if (SteamManager.Initialized) {
                    SteamRemoteStorage.FileWrite(FileName, encrypted, encrypted.Length);
                }
            }
            catch (Exception e) {
                Debug.LogWarning($"Steam cloud upload failed: {e.Message}");
            }
        }
        catch (Exception e) {
            Debug.LogError($"Save failed: {e.Message}");
        }

        CloudLogger.instance.SendGameLog();
    }

    // 외부에서 호출: 불러오기
    public static SaveData Load()
    {
        byte[] encrypted = null;

        // 1) 클라우드 우선 시도
        if (SteamManager.Initialized) {
            try {
                if (SteamRemoteStorage.FileExists(FileName)) {
                    int size = SteamRemoteStorage.GetFileSize(FileName);
                    if (size > 0) {
                        encrypted = new byte[size];
                        int read = SteamRemoteStorage.FileRead(FileName, encrypted, size);
                        if (read != size) {
                            Debug.LogWarning($"SteamRemoteStorage.FileRead read {read}/{size} bytes.");
                        }
                    }
                }
            }
            catch (Exception e) {
                Debug.LogWarning($"Steam cloud read failed: {e.Message}");
                encrypted = null;
            }
        }

        // 2) 클라우드 실패 시 로컬 파일 fallback
        if (encrypted == null && File.Exists(LocalPath)) {
            try {
                encrypted = File.ReadAllBytes(LocalPath);
            }
            catch (Exception e) {
                Debug.LogWarning($"Local read failed: {e.Message}");
                encrypted = null;
            }
        }

        // 3) 없으면 새 세이브
        if (encrypted == null || encrypted.Length == 0)
            return new SaveData();

        // 복호화 시도
        try {
            string json = Decrypt(encrypted);
            SaveData data = JsonUtility.FromJson<SaveData>(json);
            return data ?? throw new Exception("Deserialized SaveData is null");
        }
        catch (Exception e) {
            Debug.LogWarning($"Primary decrypt/load failed: {e.Message}");

            // 백업 복원 시도
            if (File.Exists(BackupPath)) {
                try {
                    byte[] bak = File.ReadAllBytes(BackupPath);
                    string json = Decrypt(bak);
                    SaveData data = JsonUtility.FromJson<SaveData>(json);
                    if (data != null) {
                        // 복원된 백업을 로컬 및 클라우드에 덮어쓰기
                        File.WriteAllBytes(LocalPath, bak);
                        try {
                            if (SteamManager.Initialized)
                                SteamRemoteStorage.FileWrite(FileName, bak, bak.Length);
                        }
                        catch { }
                        return data;
                    }
                }
                catch (Exception ex) {
                    Debug.LogError($"Backup restore failed: {ex.Message}");
                }
            }

            return new SaveData();
        }
    }

    // 암호화: [IV(16)] + CipherText (바이너리)
    private static byte[] Encrypt(string plainText)
    {
        using Aes aes = Aes.Create();
        aes.Key = GetKey();
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;
        aes.GenerateIV();

        using MemoryStream ms = new();
        // 앞에 IV 기록
        ms.Write(aes.IV, 0, aes.IV.Length);

        using (CryptoStream cs = new(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
        using (StreamWriter sw = new(cs, Encoding.UTF8)) {
            sw.Write(plainText);
        }

        return ms.ToArray();
    }

    // 복호화: encryptedData는 [IV(16)] + CipherText
    private static string Decrypt(byte[] encryptedData)
    {
        if (encryptedData == null || encryptedData.Length <= 16)
            throw new ArgumentException("Encrypted data is null or too short.");

        using Aes aes = Aes.Create();
        aes.Key = GetKey();
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;

        byte[] iv = new byte[16];
        Array.Copy(encryptedData, 0, iv, 0, iv.Length);
        aes.IV = iv;

        using MemoryStream ms = new(encryptedData, iv.Length, encryptedData.Length - iv.Length);
        using CryptoStream cs = new(ms, aes.CreateDecryptor(), CryptoStreamMode.Read);
        using StreamReader sr = new(cs, Encoding.UTF8);
        return sr.ReadToEnd();
    }

    // 테스트/디버그용: 세이브 삭제(로컬 + 백업 + 클라우드)
    public static void DeleteAllSaves()
    {
        try {
            if (File.Exists(LocalPath)) File.Delete(LocalPath);
            if (File.Exists(BackupPath)) File.Delete(BackupPath);
            if (SteamManager.Initialized && SteamRemoteStorage.FileExists(FileName)) {
                SteamRemoteStorage.FileDelete(FileName);
            }
        }
        catch (Exception e) {
            Debug.LogWarning($"DeleteAllSaves failed: {e.Message}");
        }
    }



    private const string FileName1 = "achi.dat";
    private static readonly string LocalPath1 = Path.Combine(Application.persistentDataPath, FileName1);
    private static readonly string BackupPath1 = Path.Combine(Application.persistentDataPath, "achi.bak");

    // 저장 (암호화)
    public static void SaveAchi(SaveDataAchi data)
    {
        try {
            string json = JsonUtility.ToJson(data);
            byte[] encrypted = Encrypt(json);

            // 로컬 저장
            File.WriteAllBytes(LocalPath1, encrypted);

            // 백업
            File.Copy(LocalPath1, BackupPath1, true);

            // Steam 클라우드
            try {
                if (SteamManager.Initialized) {
                    SteamRemoteStorage.FileWrite(FileName1, encrypted, encrypted.Length);
                }
            }
            catch (Exception e) {
                Debug.LogWarning($"Steam cloud upload failed: {e.Message}");
            }
        }
        catch (Exception e) {
            Debug.LogError($"SaveAchi failed: {e.Message}");
        }
    }


    // 불러오기 (복호화)
    public static SaveDataAchi LoadAchi()
    {
        byte[] encrypted = null;

        // 1) Steam 클라우드 우선
        if (SteamManager.Initialized) {
            try {
                if (SteamRemoteStorage.FileExists(FileName1)) {
                    int size = SteamRemoteStorage.GetFileSize(FileName1);
                    if (size > 0) {
                        encrypted = new byte[size];
                        int read = SteamRemoteStorage.FileRead(FileName1, encrypted, size);
                        if (read != size)
                            Debug.LogWarning($"Steam read {read}/{size}");
                    }
                }
            }
            catch (Exception e) {
                Debug.LogWarning($"Steam cloud read failed: {e.Message}");
                encrypted = null;
            }
        }

        // 2) 로컬 fallback
        if (encrypted == null && File.Exists(LocalPath1)) {
            try {
                encrypted = File.ReadAllBytes(LocalPath1);
            }
            catch (Exception e) {
                Debug.LogWarning($"Local read failed: {e.Message}");
                encrypted = null;
            }
        }

        // 3) 없으면 새 데이터
        if (encrypted == null || encrypted.Length == 0)
            return new SaveDataAchi();

        // 4) 복호화 시도
        try {
            string json = Decrypt(encrypted);
            return JsonUtility.FromJson<SaveDataAchi>(json) ?? new SaveDataAchi();
        }
        catch (Exception e) {
            Debug.LogWarning($"Achi decrypt failed: {e.Message}");

            // 5) 백업 복원
            if (File.Exists(BackupPath1)) {
                try {
                    byte[] bak = File.ReadAllBytes(BackupPath1);
                    string json = Decrypt(bak);
                    SaveDataAchi data = JsonUtility.FromJson<SaveDataAchi>(json);

                    if (data != null) {
                        File.WriteAllBytes(LocalPath1, bak);
                        if (SteamManager.Initialized)
                            SteamRemoteStorage.FileWrite(FileName1, bak, bak.Length);

                        return data;
                    }
                }
                catch (Exception ex) {
                    Debug.LogError($"Achi backup restore failed: {ex.Message}");
                }
            }

            return new SaveDataAchi();
        }
    }
}




/*public static class SaveManager
{
    private const string FileName = "save.dat";
    private static readonly string LocalPath = Path.Combine(Application.persistentDataPath, FileName);
    private static readonly string BackupPath = Path.Combine(Application.persistentDataPath, "save.bak");

    // 저장
    public static void Save(SaveData data)
    {
        try {
            string json = JsonUtility.ToJson(data);

            // 로컬 저장
            File.WriteAllText(LocalPath, json);

            // 백업 생성
            File.WriteAllText(BackupPath, json);

            // Steam 클라우드 업로드
            try {
                if (SteamManager.Initialized) {
                    byte[] bytes = System.Text.Encoding.UTF8.GetBytes(json);
                    SteamRemoteStorage.FileWrite(FileName, bytes, bytes.Length);

                    GameManager.instance.saveCheckPanel.SetActive(true);
                }
            }
            catch (Exception e) {
                Debug.LogWarning($"Steam cloud upload failed: {e.Message}");
            }
        }
        catch (Exception e) {
            Debug.LogError($"Save failed: {e.Message}");
        }
    }

    // 불러오기
    public static SaveData Load()
    {
        string json = null;

        // 1) Steam 클라우드 우선
        if (SteamManager.Initialized) {
            try {
                if (SteamRemoteStorage.FileExists(FileName)) {
                    int size = SteamRemoteStorage.GetFileSize(FileName);
                    if (size > 0) {
                        byte[] buffer = new byte[size];
                        int read = SteamRemoteStorage.FileRead(FileName, buffer, size);
                        if (read == size)
                            json = System.Text.Encoding.UTF8.GetString(buffer);
                        else
                            Debug.LogWarning($"SteamRemoteStorage.FileRead read {read}/{size} bytes.");
                    }
                }
            }
            catch (Exception e) {
                Debug.LogWarning($"Steam cloud read failed: {e.Message}");
                json = null;
            }
        }

        // 2) 로컬 fallback
        if (json == null && File.Exists(LocalPath)) {
            try {
                json = File.ReadAllText(LocalPath);
            }
            catch (Exception e) {
                Debug.LogWarning($"Local read failed: {e.Message}");
                json = null;
            }
        }

        // 3) 없으면 새 세이브
        if (string.IsNullOrEmpty(json))
            return new SaveData();

        try {
            return JsonUtility.FromJson<SaveData>(json) ?? new SaveData();
        }
        catch (Exception e) {
            Debug.LogWarning($"JSON deserialize failed: {e.Message}");

            // 백업 복원
            if (File.Exists(BackupPath)) {
                try {
                    json = File.ReadAllText(BackupPath);
                    SaveData data = JsonUtility.FromJson<SaveData>(json);
                    if (data != null) {
                        // 복원된 백업으로 로컬/클라우드 덮어쓰기
                        File.WriteAllText(LocalPath, json);
                        if (SteamManager.Initialized) {
                            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(json);
                            SteamRemoteStorage.FileWrite(FileName, bytes, bytes.Length);
                        }
                        return data;
                    }
                }
                catch (Exception ex) {
                    Debug.LogError($"Backup restore failed: {ex.Message}");
                }
            }

            return new SaveData();
        }
    }

    // 로컬 + 백업 + 클라우드 삭제
    public static void DeleteAllSaves()
    {
        try {
            if (File.Exists(LocalPath)) File.Delete(LocalPath);
            if (File.Exists(BackupPath)) File.Delete(BackupPath);
            if (SteamManager.Initialized && SteamRemoteStorage.FileExists(FileName)) {
                SteamRemoteStorage.FileDelete(FileName);
            }
        }
        catch (Exception e) {
            Debug.LogWarning($"DeleteAllSaves failed: {e.Message}");
        }
    }






    private const string FileName1 = "achi.dat";
    private static readonly string LocalPath1 = Path.Combine(Application.persistentDataPath, FileName1);
    private static readonly string BackupPath1 = Path.Combine(Application.persistentDataPath, "achi.bak");

    // 저장
    public static void SaveAchi(SaveDataAchi data)
    {
        try {
            string json = JsonUtility.ToJson(data);

            // 로컬 저장
            File.WriteAllText(LocalPath1, json);

            // 백업 생성
            File.WriteAllText(BackupPath1, json);

            // Steam 클라우드 업로드
            try {
                if (SteamManager.Initialized) {
                    byte[] bytes = System.Text.Encoding.UTF8.GetBytes(json);
                    SteamRemoteStorage.FileWrite(FileName1, bytes, bytes.Length);
                }
            }
            catch (Exception e) {
                Debug.LogWarning($"Steam cloud upload failed: {e.Message}");
            }
        }
        catch (Exception e) {
            Debug.LogError($"Save failed: {e.Message}");
        }
    }

    // 불러오기
    public static SaveDataAchi LoadAchi()
    {
        string json = null;

        // 1) Steam 클라우드 우선
        if (SteamManager.Initialized) {
            try {
                if (SteamRemoteStorage.FileExists(FileName1)) {
                    int size = SteamRemoteStorage.GetFileSize(FileName1);
                    if (size > 0) {
                        byte[] buffer = new byte[size];
                        int read = SteamRemoteStorage.FileRead(FileName1, buffer, size);
                        if (read == size)
                            json = System.Text.Encoding.UTF8.GetString(buffer);
                        else
                            Debug.LogWarning($"SteamRemoteStorage.FileRead read {read}/{size} bytes.");
                    }
                }
            }
            catch (Exception e) {
                Debug.LogWarning($"Steam cloud read failed: {e.Message}");
                json = null;
            }
        }

        // 2) 로컬 fallback
        if (json == null && File.Exists(LocalPath1)) {
            try {
                json = File.ReadAllText(LocalPath1);
            }
            catch (Exception e) {
                Debug.LogWarning($"Local read failed: {e.Message}");
                json = null;
            }
        }

        // 3) 없으면 새 세이브
        if (string.IsNullOrEmpty(json))
            return new SaveDataAchi();

        try {
            return JsonUtility.FromJson<SaveDataAchi>(json) ?? new SaveDataAchi();
        }
        catch (Exception e) {
            Debug.LogWarning($"JSON deserialize failed: {e.Message}");

            // 백업 복원
            if (File.Exists(BackupPath1)) {
                try {
                    json = File.ReadAllText(BackupPath1);
                    SaveDataAchi data = JsonUtility.FromJson<SaveDataAchi>(json);
                    if (data != null) {
                        // 복원된 백업으로 로컬/클라우드 덮어쓰기
                        File.WriteAllText(LocalPath1, json);
                        if (SteamManager.Initialized) {
                            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(json);
                            SteamRemoteStorage.FileWrite(FileName1, bytes, bytes.Length);
                        }
                        return data;
                    }
                }
                catch (Exception ex) {
                    Debug.LogError($"Backup restore failed: {ex.Message}");
                }
            }

            return new SaveDataAchi();
        }
    }
}*/
