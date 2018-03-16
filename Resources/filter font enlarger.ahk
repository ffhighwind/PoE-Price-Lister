
If (A_AhkVersion <= "1.1.23")
{
    msgbox, You need AutoHotkey v1.1.23 or later to run this script. `n`nPlease go to http://ahkscript.org/download and download a recent version.
    exit
}
  
/* You can customize the hotkeys to your liking using the following
    ! ALT, ^ CTRL, + SHIFT, # WIN
    <! (< left key, > right key)
    & means combine keys
    * means any ALT/CTRL/SHIFT
    ~ means pass input through
    & means combine keys
    Up means fired on release
   
    Example Hotkey:
    <!#n Up:: /* call when n is released and left alt is down */
    return
*/

filtersIn := ["highwind's_filter.filter", "highwind's_mapping_filter.filter", "highwind's_strict_filter.filter", "highwind's_very_strict_filter.filter"]
filtersOut := ["highwind's_filter_large.filter", "highwind's_mapping_filter_large.filter", "highwind's_strict_filter_large.filter", "highwind's_very_strict_filter_large.filter"]

Loop, 4
{
    GenerateFilter(filtersIn[A_Index], filtersOut[A_Index])
}
MsgBox Output complete.

RegExReplace(haystack, needle, replacement, count, limit, startPos)

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