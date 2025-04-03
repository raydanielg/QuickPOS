using System.Management;

public class HardwareInfo
{
    // Get Processor ID
    public static string GetProcessorId()
    {
        string processorId = string.Empty;
        ManagementObjectSearcher searcher = new ManagementObjectSearcher("select ProcessorId from Win32_Processor");

        foreach (ManagementObject obj in searcher.Get())
        {
            processorId = obj["ProcessorId"].ToString();
        }

        return processorId;
    }

    // Get Motherboard Serial Number
    public static string GetMotherboardSerialNumber()
    {
        string motherboardSerialNumber = string.Empty;
        ManagementObjectSearcher searcher = new ManagementObjectSearcher("select SerialNumber from Win32_BaseBoard");

        foreach (ManagementObject obj in searcher.Get())
        {
            motherboardSerialNumber = obj["SerialNumber"].ToString();
        }

        return motherboardSerialNumber;
    }

    // Get BIOS Serial Number
    public static string GetBiosSerialNumber()
    {
        string biosSerialNumber = string.Empty;
        ManagementObjectSearcher searcher = new ManagementObjectSearcher("select SerialNumber from Win32_BIOS");

        foreach (ManagementObject obj in searcher.Get())
        {
            biosSerialNumber = obj["SerialNumber"].ToString();
        }

        return biosSerialNumber;
    }

    // Get Hard Drive Serial Number
    public static string GetHardDriveSerialNumber()
    {
        string hardDriveSerialNumber = string.Empty;
        ManagementObjectSearcher searcher = new ManagementObjectSearcher("select SerialNumber from Win32_DiskDrive");

        foreach (ManagementObject obj in searcher.Get())
        {
            hardDriveSerialNumber = obj["SerialNumber"]?.ToString();
        }

        return hardDriveSerialNumber;
    }

    // Combine all the above information to form a unique Hardware ID
    public static string GetHardwareId()
    {
        string processorId = GetProcessorId();
        string motherboardSerialNumber = GetMotherboardSerialNumber();
        string biosSerialNumber = GetBiosSerialNumber();
        string hardDriveSerialNumber = GetHardDriveSerialNumber();

        // Combine all identifiers to generate a unique Hardware ID
        return $"{processorId}-{motherboardSerialNumber}-{biosSerialNumber}-{hardDriveSerialNumber}";
    }
}
