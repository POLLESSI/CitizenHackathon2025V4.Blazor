Write-Host "🔍 Searching for 'dotnet' and 'iisexpress' processes'..."

$targets = @('dotnet', 'iisexpress', 'CitizenHackathon2025.API') # Add the exact name of your .exe here if known

foreach ($name in $targets) {
    $procs = Get-Process -Name $name -ErrorAction SilentlyContinue
    foreach ($proc in $procs) {
        try {
            Write-Host "🛑 Closure of $($proc.ProcessName) (PID $($proc.Id))..."
            Stop-Process -Id $proc.Id -Force
        }
        catch {
            Write-Warning "❌ Impossible to kill $($proc.ProcessName): $_"
        }
    }
}

Write-Host "✅ All targeted processes have been completed (if present)."