
If (A_AhkVersion <= "1.1.23")
{
    msgbox, You need AutoHotkey v1.1.23 or later to run this script. `n`nPlease go to http://ahkscript.org/download and download a recent version.
    exit
}

filterFiles := ["highwind's_filter.filter", "highwind's_mapping_filter.filter", "highwind's_strict_filter.filter", "highwind's_very_strict_filter.filter", "highwind's_filter_large.filter", "highwind's_mapping_filter_large.filter", "highwind's_strict_filter_large.filter", "highwind's_very_strict_filter_large.filter"]

global filterChecks := ["SetTextColor", "SetBackgroundColor", "SetBorderColor", "PlayAlertSound"]

Loop, % filterFiles.MaxIndex()
{
    ValidateFilters(filterFiles[A_Index])
}
MsgBox Output complete.

ValidateFilters(filein)
{
    dict := {}

    Loop, Read, % filein
    {
        startline := ""
        linenum := A_Index
        if ErrorLevel
        {
            MsgBox % "Can't open """ . filein . " for reading."
            ErrorLevel := 0
            return
        }
        Loop, Parse, A_LoopReadLine, `n, % " `t`r"
        {
            /*
            foundpos := RegExMatch(A_LoopField, "#?\s*(Show|Hide) \s*# ")
            if(foundpos > 0)
            {
                startline := A_LoopField
                continue
            }
            */
            foundpos := RegExMatch(A_LoopField, "(SetTextColor|SetBackgroundColor|SetBorderColor|PlayAlertSound) [^#]+#.+", match)
            if ErrorLevel
            {
                MsgBox % "Error in regular expression string: " . ErrorLevel
                ErrorLevel := 0
                return
            }
            if(foundpos > 0)
            {
                arr := StrSplit(match, "#")
                if(arr.MaxIndex() > 1)
                {
                    key2 := ""
                    Loop, % filterChecks.MaxIndex()
                    {
                        if(InStr(match, filterChecks[A_Index]))
                        {
                            key2 := filterChecks[A_Index]
                            break
                        }
                    }
                    if(key2 = "")
                    {
                        MsgBox % "Error : " . match
                        return
                    }
                    key1 := arr[2]
                    val := arr[1]
                    if (dict.HasKey(key1))
                    {
                        if(dict[key1].HasKey(key2) and dict[key1][key2] != val)
                        {
                            MsgBox % "Error: " . filein . "(" . linenum . ")`n`n" . match . "`nexpected`n" . dict[key1][key2]
                        }
                        else
                        {
                            dict[key1][key2] := val
                        }
                    }
                    else
                    {
                        dict[key1] := { key2 : val }
                    }
                }
            }
        }
    }
}