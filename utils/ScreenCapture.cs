using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace highminded.utils;

public static class ScreenCapture
{
    public static async Task<bool> CaptureScreenAsync(string filePath)
    {
        try
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return await CaptureWindowsAsync(filePath);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return await CaptureMacAsync(filePath);
            }

            return false;
        }
        catch
        {
            return false;
        }
    }

    private static async Task<bool> CaptureWindowsAsync(string filePath)
    {
        var script = $$"""
                       Add-Type -AssemblyName System.Windows.Forms
                       Add-Type -AssemblyName System.Drawing

                       $bounds = [System.Windows.Forms.Screen]::PrimaryScreen.Bounds
                       $bitmap = New-Object System.Drawing.Bitmap $bounds.Width, $bounds.Height
                       $graphics = [System.Drawing.Graphics]::FromImage($bitmap)

                       try {
                            $graphics.CopyFromScreen($bounds.Location, [System.Drawing.Point]::Empty, $bounds.Size)
                            $bitmap.Save('{{filePath.Replace("\\", @"\\")}}', [System.Drawing.Imaging.ImageFormat]::Png)
                            Write-Output 'Success'
                       } catch {
                            Write-Output 'Error'
                       } finally {
                            $graphics.Dispose()
                            $bitmap.Dispose()
                       }
                       """;

        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "powershell.exe",
                Arguments = $"-Command \"{script}\"",
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true
            }
        };

        process.Start();
        var output = await process.StandardOutput.ReadToEndAsync();
        await process.WaitForExitAsync();

        return output.Trim() == "Success" && File.Exists(filePath);
    }

    private static async Task<bool> CaptureMacAsync(string filePath)
    {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "screencapture",
                Arguments = $"-x \"{filePath}\"",
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };

        process.Start();
        await process.WaitForExitAsync();

        return process.ExitCode == 0 && File.Exists(filePath);
    }
}