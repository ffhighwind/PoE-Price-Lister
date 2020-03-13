
If (A_AhkVersion <= "1.1.23")
{
    msgbox, You need AutoHotkey v1.1.23 or later to run this script. `n`nPlease go to http://ahkscript.org/download and download a recent version.
    exit
}
FileEncoding,UTF-8

filtersIn := ["S1_Regular_Highwind.filter"]

filtersIn := ["S1_Regular_Highwind.filter", "S2_Mapping_Highwind.filter", "S3_Strict_Highwind.filter", "S4_Very_Strict_Highwind.filter", "L1_Regular_Highwind.filter", "L2_Mapping_Highwind.filter", "L3_Strict_Highwind.filter", "L4_Very_Strict_Highwind.filter"]

Loop, % filtersIn.MaxIndex()
{
    ErrorLevel := 0
    GenerateFilter(filtersIn[A_Index])
}
MsgBox Output complete.

GenerateFilter(filein)
{
    FileRead, file_text, %filein%
    if ErrorLevel
    {
        MsgBox % "Can't open """ . FileName . """ for reading."
        return
    }
    outfile := FileOpen(filein, "w")
    if !IsObject(outfile)
    {
        MsgBox Can't open "%outfile%" for writing.
        return
    }
    fileLines := StrSplit(file_text, "`n")   
    state := 0
    Loop, % (fileLines.MaxIndex() - 1)
    {
        fileLine := fileLines[A_Index]
        if (state == 0) 
        {
            if (0 < RegExMatch(fileLine, "^Hide ")) 
            {
                state := 1
            }
        }
        else if (state == 1)
        {
            if (0 < RegExMatch(fileLine, "^(Show |Hide |\s+$)"))
            {
                outfile.Write("`tDisableDropSound`n")
                state := 0
            }
            else if (0 < RegExMatch(fileLine, "^\s+DisableDropSound|PlayAlertSound"))
            {
                state := 0
            }
        }
        outfile.Write(fileLine)
        outfile.Write("`n")
    }
    outfile.Close()
}