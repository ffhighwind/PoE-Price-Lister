
If (A_AhkVersion <= "1.1.23")
{
    msgbox, You need AutoHotkey v1.1.23 or later to run this script. `n`nPlease go to http://ahkscript.org/download and download a recent version.
    exit
}

filtersIn := ["S_Mapping_Highwind.filter", "S_Regular_Highwind.filter", "S_Strict_Highwind.filter", "S_Very_Strict_Highwind.filter"]
filtersOut := ["L_Mapping_Highwind.filter", "L_Regular_Highwind.filter", "L_Strict_Highwind.filter", "L_Very_Strict_Highwind.filter"]

Loop, 4
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