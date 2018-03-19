;
; FilterNova v2.1c: Item Filter Auto-Updater/Notifier
;
; Homepage: http://filterblast.oversoul.xyz/filternova.html
;
; This script uses FilterBlast API to check version of your favourite filter(s) and auto-update filter files on your PC if there is new version (or optionally only notify you about updates). The script checks for updates each hour.
; All you need to do is just configure this script by editing SETTINGS below or using Configuration UI at the tool's homepage.
;
; Special thanks to:
; /u/Eruyome87 for feedback and help with AHK gui/tooltip nuances
; /u/Greengroove for testing and feedback
; /u/NeverSinkDev for discussion, which led me to idea of notifications with links


; ######## INITIALIZATION - don't touch those lines, change only SETTINGS below

#SingleInstance, force
GroupAdd, PoEWindowGrp, Path of Exile ahk_class POEWindowClass ahk_exe PathOfExile.exe
GroupAdd, PoEWindowGrp, Path of Exile ahk_class POEWindowClass ahk_exe PathOfExileSteam.exe
GroupAdd, PoEWindowGrp, Path of Exile ahk_class POEWindowClass ahk_exe PathOfExile_x64.exe
GroupAdd, PoEWindowGrp, Path of Exile ahk_class POEWindowClass ahk_exe PathOfExile_x64Steam.exe
global Filters := []



; ######## FILTERNOVA SETTINGS

global ToolTipMode := "bottom"
; Possible values: "top", "bottom" or "off".
; When you play PoE tooltips will be displayed (at the right side of the screen) instead of notification window. 
; Tooltip sometimes may cause a blink when game is fullscreen mode - this is due AutoHotKey technical limitations.
; If set to "off" tooltips will not be displayed at all and if you play PoE you will see notification window only after you close or switch from the game.


; #### REQUIRED FILTER SETTINGS

FilterKey := "ffhighwind"

; You can find it in API-Key column at this page:
; http://filterblast.oversoul.xyz/extended-list-of-filters.html

NotifyOnly := "off"

; Set this to "on" if you only want to be notified about the filter's updates, with no auto-changes to your filter files. In this case you don't need to set up other options below.

FilterFileName := "ffhighwind"

; Here you need to specify exact file name of the filter (WITHOUT ".filter" extension), which you want to auto-update.
; If file doesn't exist - it will be crated on initial update - but don't forget to select new filter in in-game options!
; For extra safety - you can add something like "AUP" to filter name, so you'll keep manually saved filter as backup version.


; #### OPTIONAL FILTER SETTINGS - keep empty quotes "" if you don't want to use any of those

; if you want to use your saved customization - you can ignore those Presets and Themes settings and use CustomizationHashKey provided below

Presets := ["Regular", "Mapping", "Strict", "Very Strict", "Regular Large", "Mapping Large", "Strict Large", "Very Strict Large"] ; keep brackets here
ColorTheme := ""
SoundTheme := ""

; Here you can specify desired Presets or Themes of your favourite filter (if it has them)
; You need to type exact name of Preset/Theme, which you can see when you select a filter at FilterBLAST (at the top of preview area).
; If you want to update few presets at once - type preset names using this formatting: ["Preset1", "Preset2"]. In this case preset name will be appended to filename.


; If you customized the filter at FilterBLAST, you can specify this parameter to apply your customization into new versions of the filter:

CustomizationHashKey := ""

; The Hash-Key is last part of the saved link (before the slash), for example it looks like: "wp31nrf".
; You can get saved link by clicking "Save/share" at the site or find it at the start of previously downloaded .filter file.


; Next 2 lines should not be edited, they add filter settings to the array
FilterSettings := { fKey: FilterKey, fOnlyNotify: NotifyOnly, fFile: FilterFileName, fPresets: Presets, fCTheme: ColorTheme, fSTheme: SoundTheme, fCustomKey: CustomizationHashKey }
Filters.Push(FilterSettings)


; #### SETTINGS FOR ADDITIONAL FILTERS

; if you want update few different filters - just uncomment those lines (remove ";"), copy them for each filter you need and fill up same way as above.

;FilterKey := ""
;NotifyOnly := "off"
;FilterFileName := ""
;Presets := [""]
;ColorTheme := ""
;SoundTheme := ""
;CustomizationHashKey := ""
;FilterSettings := { fKey: FilterKey, fOnlyNotify: NotifyOnly, fFile: FilterFileName, fPresets: Presets, fCTheme: ColorTheme, fSTheme: SoundTheme, fCustomKey: CustomizationHashKey }
;Filters.Push(FilterSettings)

FilterKey := "ffhighwind"
NotifyOnly := "off"
FilterFileName := "highwind"
Presets := ["Regular", "Mapping", "Strict", "Very Strict", "Regular Large", "Mapping Large", "Strict Large", "Very Strict Large"]
ColorTheme := ""
SoundTheme := ""
CustomizationHashKey := ""
FilterSettings := { fKey: FilterKey, fOnlyNotify: NotifyOnly, fFile: FilterFileName, fPresets: Presets, fCTheme: ColorTheme, fSTheme: SoundTheme, fCustomKey: CustomizationHashKey }
Filters.Push(FilterSettings)




; ######## MAIN CODE

global docPoE := A_MyDocuments . "\My Games\Path of Exile\"
global urlAPI := "http://filterblast.oversoul.xyz/api/"
global urlHomepage := "http://filterblast.oversoul.xyz/filternova.html"
global NovaVersion := "2.1"
global NovaTitle := "FilterNova v" . NovaVersion
global GuiEmpty := true
global btnClose := ""

Menu Tray, NoStandard
Menu Tray, Add, Reload Script, M_Reload
Menu Tray, Add, Pause Script, M_Pause
Menu Tray, Add
Menu Tray, Add, Exit FilterNova, M_Exit
Menu Tray, Add
Menu Tray, Add, Test Notifications, TestNotifications
Menu Tray, Add, Check for Filter Updates, CheckForFilterUpdates
Menu Tray, Default, Check for Filter Updates

M_Reload() {
  Reload
}
M_Exit() {
  ExitApp
}
M_Pause() {
  if (A_IsPaused) {
    Pause off
    Menu Tray, Uncheck, Pause Script
  } else {
    Menu Tray, Check, Pause Script
    Pause On
  }
}

ToolTipShow(message, prefix = "FilterNova - ") {
	if (ToolTipMode != "off") {
		CoordMode, ToolTip, Screen
		if (ToolTipMode == "top") {
			Tooltip % prefix . message, % A_ScreenWidth, 0
		} else {
			Sleep, 2000
			Tooltip %message%, % A_ScreenWidth, 0
			WinGet hwnd, ID, ahk_class tooltips_class32
			WinGetPos, , , tW, tH, ahk_id %hWnd%
			Tooltip % prefix . message, % A_ScreenWidth - tW - 65, % A_ScreenHeight - tH - 65
		}
		SetTimer, ToolTipHide, 6660
	}
}

ToolTipHide() {
	ToolTip
	SetTimer, ToolTipHide, Off
}

GuiCreate() {
	GuiEmpty := true
	Gui, NovaMsgs:-SysMenu
	Gui, NovaMsgs:Margin, 8, 5
	Gui, NovaMsgs:Font, s9, Verdana
	Gui, NovaMsgs:Add, Link,, %NovaTitle% :: <a href="%urlHomepage%">Homepage</a> | <a href="https://www.pathofexile.com/forum/view-thread/2051783">PoE forum thread</a> | <a href="https://www.patreon.com/oversoul_xyz">Patreon</a>
	Gui, NovaMsgs:Add, Button, ym vbtnClose gGuiClose, Close
	Gui, NovaMsgs:Add, Text, x8 y29, ----------------------
}

GuiClose() {
	Gui, NovaMsgs:Destroy
	GuiCreate()
}

GuiShow() {
	SetTimer, GuiActivate, Off
	if (!GuiEmpty) {
		Gui, NovaMsgs:Show, AutoSize, %NovaTitle%
		GuiControl, NovaMsgs:Focus, btnClose
		Gui, NovaMsgs:+LastFound
		WinGetPos, , , wW, , A
		GuiControlGet, cbtn, NovaMsgs:Pos, btnClose
		cX := wW - cbtnW - 11
		GuiControl, NovaMsgs:Move, btnClose, x%cX% y5
	}
}

GuiAddInfo(message, link = "") {
	GuiEmpty := false
	if (link) {
		Gui, NovaMsgs:Add, Link, , [%A_Hour%:%A_Min%] %message%`n<a href="%link%">%link%</a>
	} else {
		Gui, NovaMsgs:Add, Text, , [%A_Hour%:%A_Min%] %message%
	}
}

GuiActivate() {
	If (!WinActive("ahk_group PoEWindowGrp")) {
		GuiShow()
	}
}

NovaSays(commontext, guitext = "", link = "", tiptext = "") {
	GuiAddInfo(commontext . guitext, link)
	If (!WinActive("ahk_group PoEWindowGrp")) {
		GuiShow()
	} else {
		commontext .= tiptext
		if (guitext OR link) {
			commontext .= "`n( Alt+Tab to see details )"
		}
		ToolTipShow(commontext)
		SetTimer, GuiActivate, 1000
	}
}

TestNotifications() {
	GuiAddInfo("Window Notification Test Message")
	GuiShow()
	Sleep, 2000
	GuiAddInfo("Test Tooltips will appear in 5 seconds.`nYou may switch to Path of Exile to check out how they'll work while you're playing.")
	GuiShow()
	Sleep, 3000
	if (ToolTipMode == "off") {
		GuiAddInfo("Tooltip test skipped - tooltips are turned off in settings")
		GuiShow()
	} else {
		ToolTipShow("Tooltip Notification`nTest Message #1`nAnother test tooltip will appear soon")
		Sleep, 2000
		ToolTipShow("Tooltip Notification`nTest Message #2 - this is the last one")
	}
}

ErrorMsg(message, gohome = true) {
	if (gohome) {
		message .= "`n`nYou need to configure this script correctly by editing it with Notepad or by using Configuration UI at: `n" . urlHomepage . "`n`nDo you want to open Configuration UI webpage now?"
	}
	MsgBox 0x14, %NovaTitle%, %message%
	if (gohome) {
		IfMsgBox Yes, {
				Run, %urlHomepage%
		}
	}
	ExitApp
}

For fID, fData in Filters {
	if (!fData["fKey"]) {
		ErrorMsg("FilterKey is not specified.")
	}
	Filters[fID]["urlcheck"] := urlAPI . "CheckVersion/?format=text&nova=" . NovaVersion . "&filter=" . fData["fKey"]
	Filters[fID]["urlinfolink"] := urlAPI . "GetLink/?filter=" . fData["fKey"]
	Filters[fID]["fileinfo"] := docPoE . "FilterNovaInfo-" . fData["fKey"] . fId . ".txt"
	Filters[fID]["fileinfonew"] := docPoE . "FilterNovaNewInfo-" . fData["fKey"] . fId . ".txt"
	if (fData["fOnlyNotify"] != "on") {
		if (!fData["fFile"]) {
			ErrorMsg("FilterFileName is not specified.")
		}
		Filters[fID]["filefilter"] := docPoE . fData["fFile"] ; . ".filter"
		Filters[fID]["filefilternew"] := docPoE . "New" . fData["fFile"] ; . ".filter"
		urldownload := urlAPI . "FilterFile/?filter=" . fData["fKey"]
		if (fData["fCustomKey"]) {
			urldownload .= "&customization=" . fData["fCustomKey"]
			Filters[fID]["fPresets"] := [""]
		} else {
			if (fData["fCTheme"]) {
				urldownload .= "&colortheme=" . fData["fCTheme"]
			}
			if (fData["fSTheme"]) {
				urldownload .= "&soundtheme=" . fData["fSTheme"]
			}
		}
		Filters[fID]["urldownload"] := urldownload
	}
}
GuiCreate()
FileDelete, % docPoE . "FilterUpdater*.txt" ; remove info files of previous version
SetTimer, CheckForFilterUpdates, 3600000
SetTimer, CheckForNovaUpdates, 14400000
CheckForFilterUpdates()
CheckForNovaUpdates()

CheckForNovaUpdates() {
	urlcheck := urlAPI . "Nova/?data=version"
	tmpapidata := docPoE . "FilterNova.tmp"
	URLDownloadToFile, %urlcheck%, %tmpapidata%
	if (!ErrorLevel) {
		FileRead, ApiData, %tmpapidata%
		if (ApiData AND ApiData != NovaVersion) {
			NovaSays("New FilterNova version available: " . ApiData, ". Get it at:", urlHomepage)
		}
	}
	FileDelete, %tmpapidata%
}

CheckForFilterUpdates() {
	Sleep, 2000
	For fId, fData in Filters {
		fileinfo := fData["fileinfo"]
		fileinfonew := fData["fileinfonew"]
		URLDownloadToFile, % fData["urlcheck"], %fileinfonew%
		if (ErrorLevel) {
			ToolTipShow("Error in filter version data download/save.`nThis may happen due internet connection issues`n or restricted access to your Documents folder.")
		} else {
			UpdateFilter := 0
			FileRead, NewData, %fileinfonew%
			if (NewData == "Incorrect filter key value") {
				ErrorMsg("Incorrect FilterKey value: " . fData["fKey"])
			}
			IfNotExist, %fileinfo%
			{
				UpdateFilter := 2
			} else {
				FileRead, OldData, %fileinfo%
				if (NewData != OldData) {
					UpdateFilter := 1
				}
			}
			if (UpdateFilter) {
				UpdateSuccessful := 1		
				UpdatedInfo := ""
				if (UpdateFilter == 2) {
					UpdatedInfo .= "( initial check ) "
				}
				if (fData["fOnlyNotify"] == "on") {
					UpdatedInfo .= "New version available:`n" . NewData
				} else {
					UpdatedPresets := ""
					For fPresetNum, fPresetName in fData["fPresets"] {
						urldownload := fData["urldownload"]
						filefilter := fData["filefilter"]
						filefilternew := fData["filefilternew"]
						if (fPresetName) {
							urldownload .= "&preset=" . fPresetName
							filefilter .= "_" . fPresetName
							filefilternew .= "_" .fPresetName
						}
						filefilter .= ".filter"
						filefilternew .= ".filter"
						URLDownloadToFile, %urldownload%, %filefilternew%
						if (ErrorLevel) {
							ToolTipShow("Error in filter file download/save.`nThis may happen due internet connection issues`n or restricted access to your Documents folder.")
						} else {
							FileRead, NewFilter, %filefilternew%
							if (StrLen(NewFilter) < 1100) {
								UpdateSuccessful := 0
								ToolTipShow(fData["fKey"] . " " . fPresetName . " - file NOT updated:`n something wrong with downloaded filter's code")
							} else {
								IfExist, %filefilter%
									FileDelete, %filefilter%
								FileMove, %filefilternew%, %filefilter%
								if (fPresetName) {
									if (UpdatedPresets) {
										UpdatedPresets := UpdatedPresets . ", "
									}
									UpdatedPresets := UpdatedPresets . fPresetName
								}
							}
							NewFilter := ""
						}
					}
					if (UpdatedPresets) {
						UpdatedPresets := UpdatedPresets . " preset(s) of`n"
					}
					UpdatedInfo .= "Successfully updated:`n" . UpdatedPresets . NewData
				}
				if (UpdateSuccessful) {
					urlinfolink := fData["urlinfolink"]
					URLDownloadToFile, %urlinfolink%, %fileinfo%
					FileRead, urlinfolink, %fileinfo%
					FileDelete, %fileinfo%
					FileMove, %fileinfonew%, %fileinfo%
					NovaSays(UpdatedInfo, "`n`nCheck out update details at:", urlinfolink)
				}
			} else {
				FileDelete, %fileinfonew%
			}
		}
	}
}