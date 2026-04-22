$folderPath = "c:\Users\DELL\source\repos\CinemaManagementSystem\Cinema.Web\Areas\NhanVien"
$files = Get-ChildItem -Path $folderPath -Recurse -Include *.cs, *.cshtml

foreach ($file in $files) {
    $content = Get-Content $file.FullName -Raw
    $newContent = $content -replace 'namespace Cinema\.Web\.Areas\.Admin', 'namespace Cinema.Web.Areas.NhanVien'
    $newContent = $newContent -replace '\[Area\("Admin"\)\]', '[Area("NhanVien")]'
    $newContent = $newContent -replace '\[AdminAuthorize\]', '[NhanVienAuthorize]'
    $newContent = $newContent -replace 'class AdminAuthorize', 'class NhanVienAuthorize'
    $newContent = $newContent -replace 'public AdminAuthorize', 'public NhanVienAuthorize'
    $newContent = $newContent -replace 'area = "Admin"', 'area = "NhanVien"'
    $newContent = $newContent -replace 'asp-area="Admin"', 'asp-area="NhanVien"'
    $newContent = $newContent -replace '/Admin/', '/NhanVien/'
    $newContent = $newContent -replace 'Trang Quản Trị', 'Trang Nhân Viên'
    
    if ($content -cne $newContent) {
        Set-Content -Path $file.FullName -Value $newContent
        Write-Host "Updated $($file.FullName)"
    }
}
