
If (A_AhkVersion <= "1.1.23")
{
    msgbox, You need AutoHotkey v1.1.23 or later to run this script. `n`nPlease go to http://ahkscript.org/download and download a recent version.
    exit
}
FileEncoding,UTF-8

filtersIn := ["S_NoRares_Highwind.filter", "S1_Regular_Highwind.filter", "S2_Mapping_Highwind.filter", "S3_Strict_Highwind.filter", "S4_Very_Strict_Highwind.filter"]
filtersOut := ["L_NoRares_Highwind.filter", "L1_Regular_Highwind.filter", "L2_Mapping_Highwind.filter", "L3_Strict_Highwind.filter", "L4_Very_Strict_Highwind.filter"]

Loop, % filtersIn.MaxIndex()
{
    GenerateFilter(filtersIn[A_Index], filtersOut[A_Index])
}
MsgBox Output complete.

GenerateFilter(filein, fileout)
{
    FileRead, file_text, %filein%
    if ErrorLevel
    {
        MsgBox % "Can't open """ . FileName . " for reading."
        return
    }
    outfile := FileOpen(fileout, "w")
    if !IsObject(outfile)
    {
        MsgBox Can't open "%outfile%" for writing.
        return
    }
    
    file_text := StrReplace(file_text, "SetFontSize 40", "SetFontSize 45")
    file_text := StrReplace(file_text, "SetFontSize 36", "SetFontSize 40")
    file_text := StrReplace(file_text, "SetFontSize 32", "SetFontSize 36")
    
    ;file_text = RegExReplace(file_text, "SetFontSize 40", "SetFontSize 45")
    ;file_text = RegExReplace(file_text, "SetFontSize 36", "SetFontSize 40")
    ;file_text = RegExReplace(file_text, "SetFontSize 32", "SetFontSize 36")
    outfile.Write(file_text)
    
    outfile.Close()
    ErrorLevel = 0
}