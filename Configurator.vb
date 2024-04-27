Imports System.ComponentModel
Imports System.Runtime.CompilerServices
Imports System.Runtime.InteropServices
Imports System.Reflection
Imports System.Net
Imports System.Net.NetworkInformation
Imports System.Net.Sockets
Imports System.Threading
Imports System.Formats.Tar
Imports System.IO

Public Class Configurator
    Public OpenIPCIP As String
    Public NVRIP As String
    Public RadxaIP As String
    Private Sub btnGet_Click(sender As Object, e As EventArgs) Handles btnGet.Click
        Dim settingsconf As String = "settings.conf"
        If Not IO.File.Exists(settingsconf) Then
            System.IO.File.Create(settingsconf).Dispose()
            Dim fileExists As Boolean = File.Exists(settingsconf)
            Using sw As New StreamWriter(File.Open(settingsconf, FileMode.OpenOrCreate))
                sw.WriteLine("openipc:192.168.0.1")
                sw.WriteLine("nvr:192.168.0.1")
                sw.WriteLine("radxa:192.168.0.1")
            End Using
            MsgBox("File " + settingsconf + " not found and default created!")
        End If

        Dim x As Integer
        Dim SettingsfilePath = settingsconf
        Dim extern = "extern.bat"
        If Not IO.File.Exists(extern) Then
            MsgBox("File " + extern + " not found!")
            Return
        End If

        If IsValidIP(txtIP.Text) Then
            With New Process()
                .StartInfo.UseShellExecute = False
                .StartInfo.FileName = extern
                Dim lines = IO.File.ReadAllLines(SettingsfilePath)
                For x = 0 To lines.Count() - 1
                    If rBtnNVR.Checked Then
                        If lines(x).StartsWith("nvr:") Then
                            lines(x) = "nvr:" + txtIP.Text
                        End If
                        .StartInfo.Arguments = "dlvrx " + String.Format("{0}", txtIP.Text) + " " + txtPassword.Text
                    ElseIf rBtnRadxaZero3w.Checked Then
                        If lines(x).StartsWith("radxa:") Then
                            lines(x) = "radxa:" + txtIP.Text
                        End If
                        .StartInfo.Arguments = "dlwfbng " + String.Format("{0}", txtIP.Text) + " " + txtPassword.Text
                    Else
                        If lines(x).StartsWith("openipc:") Then
                            lines(x) = "openipc:" + txtIP.Text
                        End If
                        .StartInfo.Arguments = "dl " + String.Format("{0}", txtIP.Text) + " " + txtPassword.Text
                    End If
                Next
                IO.File.WriteAllLines(SettingsfilePath, lines)
                .StartInfo.RedirectStandardOutput = False
                .Start()
            End With
        Else
            MsgBox("Please enter a valid IP address")
        End If
    End Sub

    Private Sub btnSend_Click(sender As Object, e As EventArgs) Handles btnSend.Click
        Dim extern = "extern.bat"
        If Not System.IO.File.Exists(extern) Then
            MsgBox("File " + extern + " not found!")
            Return
        End If

        If IsValidIP(txtIP.Text) Then
            With New Process()
                .StartInfo.UseShellExecute = False
                .StartInfo.FileName = extern
                If rBtnNVR.Checked Then
                    .StartInfo.Arguments = "ulvrx " + String.Format("{0}", txtIP.Text) + " " + txtPassword.Text
                ElseIf rBtnRadxaZero3w.Checked Then
                    .StartInfo.Arguments = "ulwfbng " + String.Format("{0}", txtIP.Text) + " " + txtPassword.Text
                Else
                    .StartInfo.Arguments = "ul " + String.Format("{0}", txtIP.Text) + " " + txtPassword.Text
                End If
                .StartInfo.RedirectStandardOutput = False
                .Start()
            End With
        Else
            MsgBox("Please enter a valid IP address")
        End If
    End Sub

    Public Function ReadLine(lineNumber As Integer, lines As List(Of String)) As String
        On Error GoTo err1
        Return lines(lineNumber - 1)
        Exit Function
err1:
    End Function

    Private Sub txtSaveFreq_Click(sender As Object, e As EventArgs) Handles txtSaveFreq.Click
        If txtFrequency.Text <> "" Then
            Dim wfbconf = "wfb.conf"
            If Not IO.File.Exists(wfbconf) Then
                MsgBox("File " + wfbconf + " not found!")
                Return
            End If
            Dim x As Integer
            Dim WFBfilePath = wfbconf
            Dim lines = IO.File.ReadAllLines(WFBfilePath)
            For x = 0 To lines.Count() - 1
                If rBtnRadxaZero3w.Checked Then
                    Dim wfbng = "wifibroadcast.cfg"
                    If Not IO.File.Exists(wfbng) Then
                        MsgBox("File " + wfbng + " not found!")
                        Return
                    End If

                    Dim wfbngfilePath = wfbng
                    Dim WFBlines = IO.File.ReadAllLines(wfbngfilePath)
                    If WFBlines(1).StartsWith("wifi_channel = ") Then
                        WFBlines(1) = txtFrequency.Text
                    End If
                    If WFBlines(7).StartsWith("peer = 'connect://") Then
                        WFBlines(7) = txtMCS.Text
                    End If
                    If WFBlines(11).StartsWith("peer = 'connect://") Then
                        WFBlines(11) = txtSTBC.Text
                    End If
                    IO.File.WriteAllLines(wfbngfilePath, WFBlines)
                    If lines(x).StartsWith("rtw_tx_pwr_idx_override=") Then
                        lines(x) = txtPower.Text
                    End If
                Else
                    If lines(x).StartsWith("channel=") Then
                        lines(x) = txtFrequency.Text
                    End If
                    If lines(x).StartsWith("driver_txpower_override=") Then
                        lines(x) = txtPower.Text
                    End If
                    If lines(x).StartsWith("frequency=") Then
                        lines(x) = txtFreq24.Text
                    End If
                    If lines(x).StartsWith("txpower=") Then
                        lines(x) = txtPower24.Text
                    End If
                    If rBtnNVR.Checked Then
                        If lines(x).StartsWith("udp_addr=") Then
                            lines(x) = txtMCS.Text
                        End If
                        If lines(x).StartsWith("udp_port=") Then
                            lines(x) = txtSTBC.Text
                        End If
                    Else
                        If lines(x).StartsWith("stbc=") Then
                            lines(x) = txtSTBC.Text
                        End If
                        If lines(x).StartsWith("ldpc=") Then
                            lines(x) = txtLDPC.Text
                        End If
                        If lines(x).StartsWith("mcs_index=") Then
                            lines(x) = txtMCS.Text
                        End If
                        If lines(x).StartsWith("fec_k=") Then
                            lines(x) = txtFECK.Text
                        End If
                        If lines(x).StartsWith("fec_n=") Then
                            lines(x) = txtFECN.Text
                        End If
                    End If
                End If
            Next
            IO.File.WriteAllLines(WFBfilePath, lines)
            MsgBox("Settings saved successfully", MsgBoxStyle.Information, "OpenIPC")
        End If
    End Sub

    Private Sub txtSaveCam_Click(sender As Object, e As EventArgs) Handles txtSaveCam.Click
        If txtResolution.Text <> "" Then
            Dim majestic = "majestic.yaml"
            If Not IO.File.Exists(majestic) Then
                MsgBox("File " + majestic + " not found!")
                Return
            End If
            Dim x As Integer
            Dim CamfilePath = majestic
            Dim lines = IO.File.ReadAllLines(CamfilePath)
            For x = 0 To lines.Count() - 1
                If lines(x).StartsWith("  contrast: ") Then
                    lines(x) = txtContrast.Text
                End If
                If lines(x).StartsWith("  hue: ") Then
                    lines(x) = txtHue.Text
                End If
                If lines(x).StartsWith("  saturation:") Then
                    lines(x) = txtSaturation.Text
                End If
                If lines(x).StartsWith("  luminance: ") Then
                    lines(x) = txtLuminance.Text
                End If
                If lines(x).StartsWith("  bitrate: ") Then
                    lines(x) = txtBitrate.Text
                End If
                If lines(x).StartsWith("  codec: ") Then
                    lines(x) = txtEncode.Text
                End If
                If lines(x).StartsWith("  size: ") Then
                    lines(x) = txtResolution.Text
                End If
                If lines(x).StartsWith("  fps: ") Then
                    lines(x) = txtFPS.Text
                End If
                If lines(x).StartsWith("  sensorConfig: ") Then
                    lines(x) = txtSensor.Text
                End If
                If lines(x).StartsWith("  exposure: ") Then
                    lines(x) = txtExposure.Text
                End If
            Next
            IO.File.WriteAllLines(CamfilePath, lines)
            MsgBox("Settings saved successfully", MsgBoxStyle.Information, "OpenIPC")
        End If
    End Sub

    Private Sub btnRead_Click(sender As Object, e As EventArgs) Handles btnRead.Click
        txtResolution.Text = ""
        txtFPS.Text = ""
        txtEncode.Text = ""
        txtBitrate.Text = ""
        txtExposure.Text = ""
        txtSaturation.Text = ""
        txtHue.Text = ""
        txtLuminance.Text = ""

        Dim wfbconf = "wfb.conf"
        If Not System.IO.File.Exists(wfbconf) Then
            MsgBox("File " + wfbconf + " not found!")
            Return
        End If
        Dim WFBreader As New IO.StreamReader(wfbconf)
        Dim WFBallLines = New List(Of String)

        Do While Not WFBreader.EndOfStream
            WFBallLines.Add(WFBreader.ReadLine)
        Loop
        WFBreader.Close()

        If rBtnNVR.Checked Then
            txtFrequency.Text = ReadLine(7, WFBallLines)
            txtPower.Text = ReadLine(10, WFBallLines)
            txtFreq24.Text = ReadLine(8, WFBallLines)
            txtPower24.Text = ReadLine(9, WFBallLines)
            txtMCS.Text = ReadLine(14, WFBallLines)
            txtSTBC.Text = ReadLine(15, WFBallLines)
        ElseIf rBtnRadxaZero3w.Checked Then
            Dim wfbng = "wifibroadcast.cfg"
            If Not System.IO.File.Exists(wfbng) Then
                MsgBox("File " + wfbng + " not found!")
                Return
            End If
            Dim WFBngreader As New IO.StreamReader(wfbng)
            Dim WFBngallLines = New List(Of String)

            Do While Not WFBngreader.EndOfStream
                WFBngallLines.Add(WFBngreader.ReadLine)
            Loop
            WFBngreader.Close()
            txtFrequency.Text = ReadLine(2, WFBngallLines)
            txtPower.Text = ReadLine(6, WFBallLines)
            txtMCS.Text = ReadLine(8, WFBngallLines)
            txtSTBC.Text = ReadLine(12, WFBngallLines)
        Else
            txtFrequency.Text = ReadLine(7, WFBallLines)
            txtPower.Text = ReadLine(10, WFBallLines)
            txtFreq24.Text = ReadLine(8, WFBallLines)
            txtPower24.Text = ReadLine(9, WFBallLines)
            txtMCS.Text = ReadLine(14, WFBallLines)
            txtSTBC.Text = ReadLine(12, WFBallLines)
            txtLDPC.Text = ReadLine(13, WFBallLines)
            txtFECK.Text = ReadLine(20, WFBallLines)
            txtFECN.Text = ReadLine(21, WFBallLines)
        End If

        If rBtnNVR.Checked Or rBtnCam.Checked Then
            Dim telemetry = "telemetry.conf"
            If Not System.IO.File.Exists(telemetry) Then
                MsgBox("File " + telemetry + " not found!")
                Return
            End If
            Dim TLMreader As New IO.StreamReader(telemetry)
            Dim TLMallLines = New List(Of String)

            Do While Not TLMreader.EndOfStream
                TLMallLines.Add(TLMreader.ReadLine)
            Loop
            TLMreader.Close()
            txtSerial.Text = ReadLine(4, TLMallLines)
            txtBaud.Text = ReadLine(5, TLMallLines)
            txtRouter.Text = ReadLine(8, TLMallLines)
            txtMCSTLM.Text = ReadLine(14, TLMallLines)
            If rBtnNVR.Checked Then
                Dim vdec = "vdec.conf"
                If Not System.IO.File.Exists(vdec) Then
                    MsgBox("File " + vdec + " not found!")
                    Return
                End If
                Dim VDECreader As New IO.StreamReader(vdec)
                Dim VDECallLines = New List(Of String)

                Do While Not VDECreader.EndOfStream
                    VDECallLines.Add(VDECreader.ReadLine)
                Loop
                VDECreader.Close()
                txtResolutionVRX.Text = ReadLine(22, VDECallLines)
                txtCodecVRX.Text = ReadLine(7, VDECallLines)
                txtFormat.Text = ReadLine(11, VDECallLines)
                txtPortVRX.Text = ReadLine(3, VDECallLines)
                txtMavlinkVRX.Text = ReadLine(26, VDECallLines)
                txtOSD.Text = ReadLine(30, VDECallLines)
                txtExtras.Text = ReadLine(52, VDECallLines)
                Dim line As String = ReadLine(53, VDECallLines)
                Dim value2 As String = line.Replace("""", "")
                Dim separators() As String = {"-osd_ele", "x", "y"}
                Dim result() As String
                result = value2.Split(separators, StringSplitOptions.RemoveEmptyEntries)
                RadioButton1.Left = result(2) / 2.5
                RadioButton1.Top = result(4) / 2.5
                RadioButton2.Left = result(6) / 2.5
                RadioButton2.Top = result(8) / 2.5
                RadioButton3.Left = result(10) / 2.5
                RadioButton3.Top = result(12) / 2.5
                RadioButton4.Left = result(14) / 2.5
                RadioButton4.Top = result(16) / 2.5
                RadioButton5.Left = result(18) / 2.5
                RadioButton5.Top = result(20) / 2.5
                RadioButton6.Left = result(22) / 2.5
                RadioButton6.Top = result(24) / 2.5
                RadioButton7.Left = result(26) / 2.5
                RadioButton7.Top = result(28) / 2.5
                RadioButton8.Left = result(30) / 2.5
                RadioButton8.Top = result(32) / 2.5
                RadioButton9.Left = result(34) / 2.5
                RadioButton9.Top = result(36) / 2.5
                RadioButton10.Left = result(38) / 2.5
                RadioButton10.Top = result(40) / 2.5
                RadioButton11.Left = result(42) / 2.5
                RadioButton11.Top = result(44) / 2.5
                RadioButton12.Left = result(46) / 2.5
                RadioButton12.Top = result(48) / 2.5
                RadioButton13.Left = result(50) / 2.5
                RadioButton13.Top = result(52) / 2.5
                RadioButton14.Left = result(54) / 2.5
                RadioButton14.Top = result(56) / 2.5
                RadioButton15.Left = result(58) / 2.5
                RadioButton15.Top = result(60) / 2.5
                RadioButton16.Left = result(62) / 2.5
                RadioButton16.Top = result(64) / 2.5
                RadioButton17.Left = result(66) / 2.5
                RadioButton17.Top = result(68) / 2.5
                RadioButton18.Left = 128 + (result(70) / 2.5)
                RadioButton18.Top = 124 + (result(72) / 2.5)
            Else
                Dim majestic = "majestic.yaml"
                If Not System.IO.File.Exists(majestic) Then
                    MsgBox("File " + majestic + " not found!")
                    Return
                End If
                Dim Camreader As New IO.StreamReader(majestic)
                Dim CamallLines = New List(Of String)

                Do While Not Camreader.EndOfStream
                    CamallLines.Add(Camreader.ReadLine)
                Loop
                Camreader.Close()
                For x = 0 To CamallLines.Count() - 1
                    If CamallLines(x).StartsWith("  size:") And txtResolution.Text = "" Then txtResolution.Text = ReadLine(x + 1, CamallLines)
                    If CamallLines(x).StartsWith("  fps:") And txtFPS.Text = "" Then txtFPS.Text = ReadLine(x + 1, CamallLines)
                    If CamallLines(x).StartsWith("  codec:") Then txtEncode.Text = ReadLine(x + 1, CamallLines)
                    If CamallLines(x).StartsWith("  bitrate:") Then txtBitrate.Text = ReadLine(x + 1, CamallLines)
                    If CamallLines(x).StartsWith("  exposure:") Then txtExposure.Text = ReadLine(x + 1, CamallLines)
                    If CamallLines(x).StartsWith("  contrast:") Then txtContrast.Text = ReadLine(x + 1, CamallLines)
                    If CamallLines(x).StartsWith("  hue:") Then txtHue.Text = ReadLine(x + 1, CamallLines)
                    If CamallLines(x).StartsWith("  saturation:") Then txtSaturation.Text = ReadLine(x + 1, CamallLines)
                    If CamallLines(x).StartsWith("  luminance:") Then txtLuminance.Text = ReadLine(x + 1, CamallLines)
                    If CamallLines(x).StartsWith("  sensorConfig:") Then txtSensor.Text = ReadLine(x + 1, CamallLines)
                Next
            End If
        Else
            Dim setdisplay = "setdisplay.sh"
            If Not System.IO.File.Exists(setdisplay) Then
                MsgBox("File " + setdisplay + " not found!")
                Return
            End If
            Dim DisplayReader As New IO.StreamReader(setdisplay)
            Dim DisplayReaderallLines = New List(Of String)

            Do While Not DisplayReader.EndOfStream
                DisplayReaderallLines.Add(DisplayReader.ReadLine)
            Loop
            DisplayReader.Close()
            txtResolutionVRX.Text = ReadLine(6, DisplayReaderallLines)
            txtCodecVRX.Text = ReadLine(7, DisplayReaderallLines)
        End If
        MsgBox("Settings loaded successfully", MsgBoxStyle.Information, "OpenIPC")
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Dim IPsettings As String = "settings.conf"
        If Not IO.File.Exists(IPsettings) Then
            System.IO.File.Create(IPsettings).Dispose()
            Dim fileExists As Boolean = File.Exists(IPsettings)
            Using sw As New StreamWriter(File.Open(IPsettings, FileMode.OpenOrCreate))
                sw.WriteLine("openipc:192.168.0.1")
                sw.WriteLine("nvr:192.168.0.1")
                sw.WriteLine("radxa:192.168.0.1")
            End Using
            MsgBox("File " + IPsettings + " not found and default created!")
        End If
        Dim IPsettingsReader As New IO.StreamReader(IPsettings)
        Dim IPsettingsReaderallLines = New List(Of String)

        Do While Not IPsettingsReader.EndOfStream
            IPsettingsReaderallLines.Add(IPsettingsReader.ReadLine)
        Loop
        IPsettingsReader.Close()
        Dim value1 As String
        Dim value2 As String
        Dim value3 As String

        value1 = ReadLine(1, IPsettingsReaderallLines)
        value2 = ReadLine(2, IPsettingsReaderallLines)
        value3 = ReadLine(3, IPsettingsReaderallLines)
        OpenIPCIP = value1.Split(":")(1)
        NVRIP = value2.Split(":")(1)
        RadxaIP = value3.Split(":")(1)
        txtIP.Text = OpenIPCIP

        ComboBox1.Items.Clear()
        ComboBox1.Items.Add("5180 MHz [36]")
        ComboBox1.Items.Add("5200 MHz [40]")
        ComboBox1.Items.Add("5220 MHz [44]")
        ComboBox1.Items.Add("5240 MHz [48]")
        ComboBox1.Items.Add("5260 MHz [52]")
        ComboBox1.Items.Add("5280 MHz [56]")
        ComboBox1.Items.Add("5300 MHz [60]")
        ComboBox1.Items.Add("5320 MHz [64]")
        ComboBox1.Items.Add("5500 MHz [100]")
        ComboBox1.Items.Add("5520 MHz [104]")
        ComboBox1.Items.Add("5540 MHz [108]")
        ComboBox1.Items.Add("5560 MHz [112]")
        ComboBox1.Items.Add("5580 MHz [116]")
        ComboBox1.Items.Add("5600 MHz [120]")
        ComboBox1.Items.Add("5620 MHz [124]")
        ComboBox1.Items.Add("5640 MHz [128]")
        ComboBox1.Items.Add("5660 MHz [132]")
        ComboBox1.Items.Add("5680 MHz [136]")
        ComboBox1.Items.Add("5700 MHz [140]")
        ComboBox1.Items.Add("5720 MHz [144]")
        ComboBox1.Items.Add("5745 MHz [149]")
        ComboBox1.Items.Add("5765 MHz [153]")
        ComboBox1.Items.Add("5785 MHz [157]")
        ComboBox1.Items.Add("5805 MHz [161]")
        ComboBox1.Items.Add("5825 MHz [165]")
        ComboBox1.Items.Add("5845 MHz [169]")
        ComboBox1.Items.Add("5865 MHz [173]")
        ComboBox1.Items.Add("5885 MHz [177]")
        ComboBox1.Text = "Select 5.8GHz Frequency"

        ComboBox2.Items.Clear()
        ComboBox2.Items.Add("20")
        ComboBox2.Items.Add("25")
        ComboBox2.Items.Add("30")
        ComboBox2.Items.Add("35")
        ComboBox2.Items.Add("40")
        ComboBox2.Items.Add("45")
        ComboBox2.Items.Add("50")
        ComboBox2.Items.Add("55")
        ComboBox2.Items.Add("58")
        ComboBox2.Text = "Select 5.8GHz TX Power"

        ComboBox3.Items.Clear()
        ComboBox3.Items.Add("2412 MHz [1]")
        ComboBox3.Items.Add("2417 MHz [2]")
        ComboBox3.Items.Add("2422 MHz [3]")
        ComboBox3.Items.Add("2427 MHz [4]")
        ComboBox3.Items.Add("2432 MHz [5]")
        ComboBox3.Items.Add("2437 MHz [6]")
        ComboBox3.Items.Add("2442 MHz [7]")
        ComboBox3.Items.Add("2447 MHz [8]")
        ComboBox3.Items.Add("2452 MHz [9]")
        ComboBox3.Items.Add("2457 MHz [10]")
        ComboBox3.Items.Add("2462 MHz [11]")
        ComboBox3.Items.Add("2467 MHz [12]")
        ComboBox3.Items.Add("2472 MHz [13]")
        ComboBox3.Items.Add("2484 MHz [14]")
        ComboBox3.Text = "Select 2.4GHz Frequency"

        ComboBox4.Items.Clear()
        ComboBox4.Items.Add("20")
        ComboBox4.Items.Add("25")
        ComboBox4.Items.Add("30")
        ComboBox4.Items.Add("35")
        ComboBox4.Items.Add("40")
        ComboBox4.Items.Add("45")
        ComboBox4.Items.Add("50")
        ComboBox4.Items.Add("55")
        ComboBox4.Items.Add("58")
        ComboBox4.Text = "Select 2.4GHz TX Power"

        ComboBox5.Items.Clear()
        ComboBox5.Items.Add("0")
        ComboBox5.Items.Add("1")
        ComboBox5.Items.Add("2")
        ComboBox5.Items.Add("3")
        ComboBox5.Items.Add("4")
        ComboBox5.Items.Add("5")
        ComboBox5.Items.Add("6")
        ComboBox5.Items.Add("7")
        ComboBox5.Items.Add("8")
        ComboBox5.Items.Add("9")
        ComboBox5.Text = "Select MCS INDEX"

        ComboBox6.Items.Clear()
        ComboBox6.Items.Add("0")
        ComboBox6.Items.Add("1")
        ComboBox6.Text = "Select STBC"

        ComboBox7.Items.Clear()
        ComboBox7.Items.Add("0")
        ComboBox7.Items.Add("1")
        ComboBox7.Text = "Select LDPC"

        ComboBox8.Items.Clear()
        ComboBox8.Items.Add("1")
        ComboBox8.Items.Add("2")
        ComboBox8.Items.Add("3")
        ComboBox8.Items.Add("4")
        ComboBox8.Items.Add("5")
        ComboBox8.Items.Add("6")
        ComboBox8.Items.Add("7")
        ComboBox8.Items.Add("8")
        ComboBox8.Items.Add("9")
        ComboBox8.Items.Add("10")
        ComboBox8.Items.Add("11")
        ComboBox8.Items.Add("12")
        ComboBox8.Text = "Select FEC K"

        ComboBox9.Items.Clear()
        ComboBox9.Items.Add("1")
        ComboBox9.Items.Add("2")
        ComboBox9.Items.Add("3")
        ComboBox9.Items.Add("4")
        ComboBox9.Items.Add("5")
        ComboBox9.Items.Add("6")
        ComboBox9.Items.Add("7")
        ComboBox9.Items.Add("8")
        ComboBox9.Items.Add("9")
        ComboBox9.Items.Add("10")
        ComboBox9.Items.Add("11")
        ComboBox9.Items.Add("12")
        ComboBox9.Text = "Select FEC N"

        cmbResolution.Items.Clear()
        cmbResolution.Items.Add("1280x720")
        cmbResolution.Items.Add("1456x816")
        cmbResolution.Items.Add("1920x1080")
        cmbResolution.Items.Add("2104x1184")
        cmbResolution.Items.Add("2240x1264")
        cmbResolution.Items.Add("2312x1304")
        cmbResolution.Items.Add("2560x1440")
        cmbResolution.Items.Add("2560x1920")
        cmbResolution.Items.Add("3200x1800")
        cmbResolution.Items.Add("3840x2160")
        cmbResolution.Text = "Select Resolution"

        cmbOSD.Items.Clear()
        cmbOSD.Items.Add("simple")
        cmbOSD.Items.Add("none")
        cmbOSD.Text = "Select OSD"

        cmbFormat.Items.Clear()
        cmbFormat.Items.Add("frame")
        cmbFormat.Items.Add("stream")
        cmbFormat.Text = "Select Format"

        cmbResolutionVRX.Items.Clear()
        cmbResolutionVRX.Items.Add("720p60")
        cmbResolutionVRX.Items.Add("1080p60")
        cmbResolutionVRX.Items.Add("1024x768x60")
        cmbResolutionVRX.Items.Add("1366x768x60")
        cmbResolutionVRX.Items.Add("1280x1024x60")
        cmbResolutionVRX.Items.Add("1600x1200x60")
        cmbResolutionVRX.Items.Add("2560x1440x30")
        cmbResolutionVRX.Text = "Select Resolution"

        cmbFPS.Items.Clear()
        cmbFPS.Items.Add("20")
        cmbFPS.Items.Add("30")
        cmbFPS.Items.Add("50")
        cmbFPS.Items.Add("59")
        cmbFPS.Items.Add("60")
        cmbFPS.Items.Add("80")
        cmbFPS.Items.Add("90")
        cmbFPS.Items.Add("100")
        cmbFPS.Items.Add("110")
        cmbFPS.Items.Add("119")
        cmbFPS.Items.Add("120")
        cmbFPS.Text = "Select FPS"

        cmbCodec.Items.Clear()
        cmbCodec.Items.Add("h264")
        cmbCodec.Items.Add("h265")
        cmbCodec.Text = "Select Codec"

        cmbCodecVRX.Items.Clear()
        cmbCodecVRX.Items.Add("h264")
        cmbCodecVRX.Items.Add("h265")
        cmbCodecVRX.Text = "Select Codec"

        cmbBitrate.Items.Clear()
        cmbBitrate.Items.Add("1024")
        cmbBitrate.Items.Add("2048")
        cmbBitrate.Items.Add("3072")
        cmbBitrate.Items.Add("4096")
        cmbBitrate.Items.Add("5120")
        cmbBitrate.Items.Add("6144")
        cmbBitrate.Items.Add("7168")
        cmbBitrate.Items.Add("8192")
        cmbBitrate.Items.Add("9216")
        cmbBitrate.Items.Add("10240")
        cmbBitrate.Items.Add("11264")
        cmbBitrate.Items.Add("12288")
        cmbBitrate.Items.Add("13312")
        cmbBitrate.Text = "Select Bitrate"

        cmbExposure.Items.Clear()
        cmbExposure.Items.Add("6")
        cmbExposure.Items.Add("8")
        cmbExposure.Items.Add("10")
        cmbExposure.Items.Add("11")
        cmbExposure.Items.Add("12")
        cmbExposure.Items.Add("16")
        cmbExposure.Items.Add("33")
        cmbExposure.Items.Add("50")
        cmbExposure.Text = "Select Exposure"

        cmbContrast.Items.Clear()
        cmbContrast.Items.Add("1")
        cmbContrast.Items.Add("5")
        cmbContrast.Items.Add("10")
        cmbContrast.Items.Add("20")
        cmbContrast.Items.Add("30")
        cmbContrast.Items.Add("40")
        cmbContrast.Items.Add("50")
        cmbContrast.Items.Add("60")
        cmbContrast.Items.Add("70")
        cmbContrast.Items.Add("80")
        cmbContrast.Items.Add("90")
        cmbContrast.Items.Add("100")
        cmbContrast.Text = "Select Contrast"

        cmbHue.Items.Clear()
        cmbHue.Items.Add("1")
        cmbHue.Items.Add("5")
        cmbHue.Items.Add("10")
        cmbHue.Items.Add("20")
        cmbHue.Items.Add("30")
        cmbHue.Items.Add("40")
        cmbHue.Items.Add("50")
        cmbHue.Items.Add("60")
        cmbHue.Items.Add("70")
        cmbHue.Items.Add("80")
        cmbHue.Items.Add("90")
        cmbHue.Items.Add("100")
        cmbHue.Text = "Select Hue"

        cmbSaturation.Items.Clear()
        cmbSaturation.Items.Add("1")
        cmbSaturation.Items.Add("5")
        cmbSaturation.Items.Add("10")
        cmbSaturation.Items.Add("20")
        cmbSaturation.Items.Add("30")
        cmbSaturation.Items.Add("40")
        cmbSaturation.Items.Add("50")
        cmbSaturation.Items.Add("60")
        cmbSaturation.Items.Add("70")
        cmbSaturation.Items.Add("80")
        cmbSaturation.Items.Add("90")
        cmbSaturation.Items.Add("100")
        cmbSaturation.Text = "Select Saturation"

        cmbLuminance.Items.Clear()
        cmbLuminance.Items.Add("1")
        cmbLuminance.Items.Add("5")
        cmbLuminance.Items.Add("10")
        cmbLuminance.Items.Add("20")
        cmbLuminance.Items.Add("30")
        cmbLuminance.Items.Add("40")
        cmbLuminance.Items.Add("50")
        cmbLuminance.Items.Add("60")
        cmbLuminance.Items.Add("70")
        cmbLuminance.Items.Add("80")
        cmbLuminance.Items.Add("90")
        cmbLuminance.Items.Add("100")
        cmbLuminance.Text = "Select Luminance"

        cmbSerial.Items.Clear()
        cmbSerial.Items.Add("/dev/ttyS0")
        cmbSerial.Items.Add("/dev/ttyS1")
        cmbSerial.Items.Add("/dev/ttyS2")
        cmbSerial.Text = "Select Serial Port"

        cmbBaud.Items.Clear()
        cmbBaud.Items.Add("4800")
        cmbBaud.Items.Add("9600")
        cmbBaud.Items.Add("19200")
        cmbBaud.Items.Add("38400")
        cmbBaud.Items.Add("57600")
        cmbBaud.Items.Add("112500")
        cmbBaud.Text = "Select Baud Rate"

        cmbRouter.Items.Clear()
        cmbRouter.Items.Add("0")
        cmbRouter.Items.Add("1")
        cmbRouter.Text = "Select MAVFWD(0)/MAVLINK(1)"

        cmbMCSTLM.Items.Clear()
        cmbMCSTLM.Items.Add("0")
        cmbMCSTLM.Items.Add("1")
        cmbMCSTLM.Items.Add("2")
        cmbMCSTLM.Items.Add("3")
        cmbMCSTLM.Items.Add("4")
        cmbMCSTLM.Items.Add("5")
        cmbMCSTLM.Items.Add("6")
        cmbMCSTLM.Items.Add("7")
        cmbMCSTLM.Items.Add("8")
        cmbMCSTLM.Items.Add("9")
        cmbMCSTLM.Text = "Select MCS INDEX"

        rBtnCam.BackColor = Color.Gold
        rBtnCam.ForeColor = Color.Black
    End Sub

    Private Sub ComboBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox1.SelectedIndexChanged
        Dim sInput = ComboBox1.SelectedItem.ToString
        Dim last4Letter = sInput.Substring(sInput.Length - 4).Replace("]", "").Replace("[", "")
        If rBtnRadxaZero3w.Checked Then
            txtFrequency.Text = "wifi_channel = " & last4Letter
        Else
            txtFrequency.Text = "channel=" & last4Letter
            txtFreq24.Text = "frequency="
        End If
    End Sub

    Private Sub ComboBox2_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox2.SelectedIndexChanged
        If rBtnRadxaZero3w.Checked Then
            txtPower.Text = "rtw_tx_pwr_idx_override=" & ComboBox2.SelectedItem.ToString
        Else
            txtPower.Text = "driver_txpower_override=" & ComboBox2.SelectedItem.ToString
        End If
    End Sub

    Private Sub ComboBox3_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox3.SelectedIndexChanged
        Dim sInput = ComboBox3.SelectedItem.ToString
        Dim last3Letter = sInput.Substring(sInput.Length - 3).Replace("]", "").Replace("[", "")
        txtFrequency.Text = "channel="
        txtFreq24.Text = "frequency=" & last3Letter
    End Sub

    Private Sub ComboBox4_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox4.SelectedIndexChanged
        txtPower24.Text = "txpower=" & ComboBox4.SelectedItem.ToString
    End Sub

    Private Sub ComboBox5_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox5.SelectedIndexChanged
        txtMCS.Text = "mcs_index=" & ComboBox5.SelectedItem.ToString
    End Sub

    Private Sub ComboBox6_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox6.SelectedIndexChanged
        txtSTBC.Text = "stbc=" & ComboBox6.SelectedItem.ToString
    End Sub

    Private Sub ComboBox7_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox7.SelectedIndexChanged
        txtLDPC.Text = "ldpc=" & ComboBox7.SelectedItem.ToString
    End Sub

    Private Sub ComboBox8_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox8.SelectedIndexChanged
        txtFECK.Text = "fec_k=" & ComboBox8.SelectedItem.ToString
    End Sub

    Private Sub ComboBox9_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox9.SelectedIndexChanged
        txtFECN.Text = "fec_n=" & ComboBox9.SelectedItem.ToString
    End Sub

    Private Sub cmbResolution_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbResolution.SelectedIndexChanged
        txtResolution.Text = "  size: " & cmbResolution.SelectedItem.ToString
        If cmbResolution.SelectedItem = "1280x720" Then
            txtFPS.Text = "  fps: 120"
            cmbFPS.Text = "120"
            txtExposure.Text = "  exposure: 8"
            cmbExposure.Text = "8"
        ElseIf cmbResolution.SelectedItem = "1456x816" Then
            txtFPS.Text = "  fps: 120"
            cmbFPS.Text = "120"
            txtExposure.Text = "  exposure: 8"
            cmbExposure.Text = "8"
        ElseIf cmbResolution.SelectedItem = "1920x1080" Then
            txtFPS.Text = "  fps: 90"
            cmbFPS.Text = "90"
            txtExposure.Text = "  exposure: 11"
            cmbExposure.Text = "11"
        ElseIf cmbResolution.SelectedItem = "2104x1184" Then
            txtFPS.Text = "  fps: 100"
            cmbFPS.Text = "100"
            txtExposure.Text = "  exposure: 10"
            cmbExposure.Text = "10"
        ElseIf cmbResolution.SelectedItem = "2240x1264" Then
            txtFPS.Text = "  fps: 60"
            cmbFPS.Text = "60"
            txtExposure.Text = "  exposure: 16"
            cmbExposure.Text = "16"
        ElseIf cmbResolution.SelectedItem = "2312x1304" Then
            txtFPS.Text = "  fps: 80"
            cmbFPS.Text = "80"
            txtExposure.Text = "  exposure: 12"
            cmbExposure.Text = "12"
        ElseIf cmbResolution.SelectedItem = "2560x1440" Then
            txtFPS.Text = "  fps: 60"
            cmbFPS.Text = "60"
            txtExposure.Text = "  exposure: 16"
            cmbExposure.Text = "16"
        ElseIf cmbResolution.SelectedItem = "2560x1920" Then
            txtFPS.Text = "  fps: 30"
            cmbFPS.Text = "30"
            txtExposure.Text = "  exposure: 33"
            cmbExposure.Text = "33"
        ElseIf cmbResolution.SelectedItem = "3200x1800" Then
            txtFPS.Text = "  fps: 30"
            cmbFPS.Text = "30"
            txtExposure.Text = "  exposure: 33"
            cmbExposure.Text = "33"
        ElseIf cmbResolution.SelectedItem = "3840x2160" Then
            txtFPS.Text = "  fps: 20"
            cmbFPS.Text = "20"
            txtExposure.Text = "  exposure: 50"
            cmbExposure.Text = "50"
        End If
    End Sub

    Private Sub cmbFPS_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbFPS.SelectedIndexChanged
        txtFPS.Text = "  fps: " & cmbFPS.SelectedItem.ToString
        txtExposure.Text = "  exposure: " & Math.Floor(1000 / CInt(cmbFPS.SelectedItem.ToString))
    End Sub

    Private Sub cmbCodec_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbCodec.SelectedIndexChanged
        txtEncode.Text = "  codec: " & cmbCodec.SelectedItem.ToString
    End Sub

    Private Sub cmbBitrate_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbBitrate.SelectedIndexChanged
        txtBitrate.Text = "  bitrate: " & cmbBitrate.SelectedItem.ToString
    End Sub

    Private Sub cmbExposure_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbExposure.SelectedIndexChanged
        txtExposure.Text = "  exposure: " & cmbExposure.SelectedItem.ToString
    End Sub

    Private Sub cmbContrast_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbContrast.SelectedIndexChanged
        txtContrast.Text = "  contrast: " & cmbContrast.SelectedItem.ToString
    End Sub

    Private Sub cmbHue_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbHue.SelectedIndexChanged
        txtHue.Text = "  hue: " & cmbHue.SelectedItem.ToString
    End Sub

    Private Sub cmbSaturation_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbSaturation.SelectedIndexChanged
        txtSaturation.Text = "  saturation: " & cmbSaturation.SelectedItem.ToString
    End Sub

    Private Sub cmbLuminance_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbLuminance.SelectedIndexChanged
        txtLuminance.Text = "  luminance: " & cmbLuminance.SelectedItem.ToString
    End Sub

    Function IsValidIP(ByVal ipAddress As String) As Boolean
        Return System.Text.RegularExpressions.Regex.IsMatch(ipAddress,
    "^(25[0-5]|2[0-4]\d|[0-1]?\d?\d)(\.(25[0-5]|2[0-4]\d|[0-1]?\d?\d)){3}$")
    End Function

    Private Sub btnReboot_Click(sender As Object, e As EventArgs) Handles btnReboot.Click
        Dim extern = "extern.bat"
        If Not File.Exists(extern) Then
            MsgBox("File " + extern + " not found!")
            Return
        End If

        If IsValidIP(txtIP.Text) Then
            With New Process()
                .StartInfo.UseShellExecute = False
                .StartInfo.FileName = extern
                .StartInfo.Arguments = "rb " + String.Format("{0}", txtIP.Text) + " " + txtPassword.Text
                .StartInfo.RedirectStandardOutput = False
                .Start()
            End With
        Else
            MsgBox("Please enter a valid IP address")
        End If
    End Sub

    Private Sub cmbSerial_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbSerial.SelectedIndexChanged
        txtSerial.Text = "serial=" & cmbSerial.SelectedItem.ToString
    End Sub

    Private Sub cmbBaud_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbBaud.SelectedIndexChanged
        txtBaud.Text = "baud=" & cmbBaud.SelectedItem.ToString
    End Sub

    Private Sub cmbRouter_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbRouter.SelectedIndexChanged
        txtRouter.Text = "router=" & cmbRouter.SelectedItem.ToString
    End Sub

    Private Sub cmbMCSTLM_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbMCSTLM.SelectedIndexChanged
        txtMCSTLM.Text = "mcs_index=" & cmbMCSTLM.SelectedItem.ToString
    End Sub

    Private Sub txtSaveTLM_Click(sender As Object, e As EventArgs) Handles txtSaveTLM.Click
        If txtSerial.Text <> "" Then
            Dim telemetry = "telemetry.conf"
            If Not System.IO.File.Exists(telemetry) Then
                MsgBox("File " + telemetry + " not found!")
                Return
            End If
            Dim x As Integer
            Dim TLMfilePath As String = telemetry
            Dim lines = IO.File.ReadAllLines(TLMfilePath)
            For x = 0 To lines.Count() - 1
                If lines(x).StartsWith("serial=") Then
                    lines(x) = txtSerial.Text
                End If
                If lines(x).StartsWith("baud=") Then
                    lines(x) = txtBaud.Text
                End If
                If lines(x).StartsWith("router=") Then
                    lines(x) = txtRouter.Text
                End If
                If lines(x).StartsWith("mcs_index=") Then
                    lines(x) = txtMCSTLM.Text
                End If
            Next
            IO.File.WriteAllLines(TLMfilePath, lines)
            MsgBox("Settings saved successfully", MsgBoxStyle.Information, "OpenIPC")
        End If
    End Sub

    Private Sub txtSaveVRX_Click(sender As Object, e As EventArgs) Handles txtSaveVRX.Click
        If txtResolutionVRX.Text <> "" Then
            If rBtnRadxaZero3w.Checked Then
                Dim setdisplay = "setdisplay.sh"
                If Not IO.File.Exists(setdisplay) Then
                    MsgBox("File " + setdisplay + " not found!")
                    Return
                End If
                Dim x, y As Integer
                Dim setdisplayfilePath = setdisplay
                Dim setdisplaylines = IO.File.ReadAllLines(setdisplayfilePath)
                For x = 0 To setdisplaylines.Count - 1
                    If setdisplaylines(x).StartsWith("MODE=") Then
                        setdisplaylines(x) = txtResolutionVRX.Text
                    End If
                    If setdisplaylines(x).StartsWith("RATE=") Then
                        setdisplaylines(x) = txtCodecVRX.Text
                    End If
                Next
                IO.File.WriteAllLines(setdisplayfilePath, setdisplaylines)
            Else
                Dim vdec = "vdec.conf"
                If Not IO.File.Exists(vdec) Then
                    MsgBox("File " + vdec + " not found!")
                    Return
                End If

                Dim VDECfilePath = vdec
                Dim lines = IO.File.ReadAllLines(VDECfilePath)
                For y = 0 To lines.Count - 1
                    If lines(y).StartsWith("mode=") Then
                        lines(y) = txtResolutionVRX.Text
                    End If
                    If lines(y).StartsWith("codec=") Then
                        lines(y) = txtCodecVRX.Text
                    End If
                    If lines(y).StartsWith("format=") Then
                        lines(y) = txtFormat.Text
                    End If
                    If lines(y).StartsWith("port=") Then
                        lines(y) = txtPortVRX.Text
                    End If
                    If lines(y).StartsWith("mavlink_port=") Then
                        lines(y) = txtMavlinkVRX.Text
                    End If
                    If lines(y).StartsWith("osd=") Then
                        lines(y) = txtOSD.Text
                    End If
                    If lines(y).StartsWith("extra=") Then
                        lines(y) = txtExtras.Text
                    End If
                Next
                IO.File.WriteAllLines(VDECfilePath, lines)
            End If
            MsgBox("Settings saved successfully", MsgBoxStyle.Information, "OpenIPC")
        End If
    End Sub

    Private Sub cmbResolutionVRX_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbResolutionVRX.SelectedIndexChanged
        If rBtnRadxaZero3w.Checked Then
            txtResolutionVRX.Text = "MODE=" & cmbResolutionVRX.SelectedItem.ToString
        Else
            txtResolutionVRX.Text = "mode=" & cmbResolutionVRX.SelectedItem.ToString
        End If
    End Sub

    Private Sub cmbCodecVRX_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbCodecVRX.SelectedIndexChanged
        If rBtnRadxaZero3w.Checked Then
            txtCodecVRX.Text = "RATE=" & cmbCodecVRX.SelectedItem.ToString
        Else
            txtCodecVRX.Text = "codec=" & cmbCodecVRX.SelectedItem.ToString
        End If
    End Sub

    Private Sub cmbOSD_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbOSD.SelectedIndexChanged
        txtOSD.Text = "osd=" & cmbOSD.SelectedItem.ToString
    End Sub

    Private Sub cmbFormat_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbFormat.SelectedIndexChanged
        txtFormat.Text = "format=" & cmbFormat.SelectedItem.ToString
    End Sub

    Private Sub btnGenerateKeys_Click(sender As Object, e As EventArgs) Handles btnGenerateKeys.Click
        Dim extern = "extern.bat"
        If Not IO.File.Exists(extern) Then
            MsgBox("File " + extern + " not found!")
            Return
        End If

        If IsValidIP(txtIP.Text) Then
            With New Process()
                .StartInfo.UseShellExecute = False
                .StartInfo.FileName = extern
                .StartInfo.Arguments = "keysgen " + String.Format("{0}", txtIP.Text) + " " + txtPassword.Text
                .StartInfo.RedirectStandardOutput = False
                .Start()
            End With
        Else
            MsgBox("Please enter a valid IP address")
        End If
    End Sub

    Private Sub btnReceiveKeys_Click(sender As Object, e As EventArgs) Handles btnReceiveKeys.Click
        Dim extern = "extern.bat"
        If Not IO.File.Exists(extern) Then
            MsgBox("File " + extern + " not found!")
            Return
        End If

        If IsValidIP(txtIP.Text) Then
            With New Process()
                .StartInfo.UseShellExecute = False
                .StartInfo.FileName = extern
                .StartInfo.Arguments = "keysdl " + String.Format("{0}", txtIP.Text) + " " + txtPassword.Text
                .StartInfo.RedirectStandardOutput = False
                .Start()
            End With
        Else
            MsgBox("Please enter a valid IP address")
        End If
    End Sub

    Private Sub btnSendKeys_Click_1(sender As Object, e As EventArgs) Handles btnSendKeys.Click
        Dim extern = "extern.bat"
        If Not IO.File.Exists(extern) Then
            MsgBox("File " + extern + " not found!")
            Return
        End If

        If IsValidIP(txtIP.Text) Then
            With New Process()
                .StartInfo.UseShellExecute = False
                .StartInfo.FileName = extern
                .StartInfo.Arguments = "keysul " + String.Format("{0}", txtIP.Text) + " " + txtPassword.Text
                .StartInfo.RedirectStandardOutput = False
                .Start()
            End With
        Else
            MsgBox("Please enter a valid IP address")
        End If
    End Sub

    Private Sub btnUpdate_Click_1(sender As Object, e As EventArgs) Handles btnUpdate.Click
        Dim extern = "extern.bat"
        If Not IO.File.Exists(extern) Then
            MsgBox("File " + extern + " not found!")
            Return
        End If

        If IsValidIP(txtIP.Text) Then
            With New Process()
                .StartInfo.UseShellExecute = False
                .StartInfo.FileName = extern
                .StartInfo.Arguments = "sysup " + String.Format("{0}", txtIP.Text) + " " + txtPassword.Text
                .StartInfo.RedirectStandardOutput = False
                .Start()
            End With
        Else
            MsgBox("Please enter a valid IP address")
        End If
    End Sub

    Private Sub btnRestartWFB_Click(sender As Object, e As EventArgs) Handles btnRestartWFB.Click
        Dim extern = "extern.bat"
        If Not IO.File.Exists(extern) Then
            MsgBox("File " + extern + " not found!")
            Return
        End If

        If IsValidIP(txtIP.Text) Then
            With New Process()
                .StartInfo.UseShellExecute = False
                .StartInfo.FileName = extern
                .StartInfo.Arguments = "rswfb " + String.Format("{0}", txtIP.Text) + " " + txtPassword.Text
                .StartInfo.RedirectStandardOutput = False
                .Start()
            End With
        Else
            MsgBox("Please enter a valid IP address")
        End If
    End Sub

    Private Sub btnRestartMajestic_Click(sender As Object, e As EventArgs) Handles btnRestartMajestic.Click
        Dim extern = "extern.bat"
        If Not IO.File.Exists(extern) Then
            MsgBox("File " + extern + " not found!")
            Return
        End If

        If IsValidIP(txtIP.Text) Then
            With New Process()
                .StartInfo.UseShellExecute = False
                .StartInfo.FileName = extern
                .StartInfo.Arguments = "rsmaj " + String.Format("{0}", txtIP.Text) + " " + txtPassword.Text
                .StartInfo.RedirectStandardOutput = False
                .Start()
            End With
        Else
            MsgBox("Please enter a valid IP address")
        End If
    End Sub

    Private Sub rBtnNVR_CheckedChanged(sender As Object, e As EventArgs) Handles rBtnNVR.CheckedChanged
        txtIP.Text = NVRIP
        Dim rb = DirectCast(sender, RadioButton)

        If rb.Checked Then
            rb.BackColor = Color.Gold
            rb.ForeColor = Color.Black
        Else
            rb.BackColor = Color.FromArgb(45, 45, 45)
            rb.ForeColor = Color.White
        End If
        Label8.Visible = True
        Label9.Visible = True
        txtResX.Visible = True
        txtResY.Visible = True
        checkCustomRes.Visible = True
        btnSendKeys.Visible = False
        btnGenerateKeys.Visible = True
        btnUpdate.Visible = False
        txtSaveVRX.Visible = True
        btnUART2.Visible = False
        btnUART2OFF.Visible = False
        btnRestartWFB.Visible = True
        btnRestartMajestic.Visible = False
        txtSaveCam.Visible = False
        txtSaveTLM.Visible = False
        ComboBox3.Visible = True
        ComboBox4.Visible = True
        ComboBox5.Visible = False
        ComboBox6.Visible = False
        ComboBox7.Visible = False
        ComboBox8.Visible = False
        ComboBox9.Visible = False
        cmbOSD.Visible = True
        cmbFormat.Visible = True
        txtLDPC.Visible = False
        txtFECK.Visible = False
        txtFECN.Visible = False
        txtOSD.Visible = True
        txtFormat.Visible = True
        txtFreq24.Visible = True
        txtPower24.Visible = True
        txtPortVRX.Visible = True
        txtMavlinkVRX.Visible = True
        txtExtras.Visible = True
        Label2.Visible = True
        txtMCS.ReadOnly = False
        txtSTBC.ReadOnly = False
        ComboBox1.Items.Clear()
        ComboBox1.Items.Add("5180 MHz [36]")
        ComboBox1.Items.Add("5200 MHz [40]")
        ComboBox1.Items.Add("5220 MHz [44]")
        ComboBox1.Items.Add("5240 MHz [48]")
        ComboBox1.Items.Add("5260 MHz [52]")
        ComboBox1.Items.Add("5280 MHz [56]")
        ComboBox1.Items.Add("5300 MHz [60]")
        ComboBox1.Items.Add("5320 MHz [64]")
        ComboBox1.Items.Add("5500 MHz [100]")
        ComboBox1.Items.Add("5520 MHz [104]")
        ComboBox1.Items.Add("5540 MHz [108]")
        ComboBox1.Items.Add("5560 MHz [112]")
        ComboBox1.Items.Add("5580 MHz [116]")
        ComboBox1.Items.Add("5600 MHz [120]")
        ComboBox1.Items.Add("5620 MHz [124]")
        ComboBox1.Items.Add("5640 MHz [128]")
        ComboBox1.Items.Add("5660 MHz [132]")
        ComboBox1.Items.Add("5680 MHz [136]")
        ComboBox1.Items.Add("5700 MHz [140]")
        ComboBox1.Items.Add("5720 MHz [144]")
        ComboBox1.Items.Add("5745 MHz [149]")
        ComboBox1.Items.Add("5765 MHz [153]")
        ComboBox1.Items.Add("5785 MHz [157]")
        ComboBox1.Items.Add("5805 MHz [161]")
        ComboBox1.Items.Add("5825 MHz [165]")
        ComboBox1.Items.Add("5845 MHz [169]")
        ComboBox1.Items.Add("5865 MHz [173]")
        ComboBox1.Items.Add("5885 MHz [177]")
        ComboBox1.Text = "Select 5.8GHz Frequency"
        cmbResolutionVRX.Items.Clear()
        cmbResolutionVRX.Items.Add("720p60")
        cmbResolutionVRX.Items.Add("1080p60")
        cmbResolutionVRX.Items.Add("1024x768x60")
        cmbResolutionVRX.Items.Add("1366x768x60")
        cmbResolutionVRX.Items.Add("1280x1024x60")
        cmbResolutionVRX.Items.Add("1600x1200x60")
        cmbResolutionVRX.Items.Add("2560x1440x30")
        cmbResolutionVRX.Text = "Select Resolution"
        cmbCodecVRX.Items.Clear()
        cmbCodecVRX.Items.Add("h264")
        cmbCodecVRX.Items.Add("h265")
        cmbCodecVRX.Text = "Select Codec"
        txtPassword.Text = "12345"
    End Sub

    Private Sub rBtnCam_CheckedChanged(sender As Object, e As EventArgs) Handles rBtnCam.CheckedChanged
        txtIP.Text = OpenIPCIP
        Dim rb = DirectCast(sender, RadioButton)

        If rb.Checked Then
            rb.BackColor = Color.Gold
            rb.ForeColor = Color.Black
        Else
            rb.BackColor = Color.FromArgb(45, 45, 45)
            rb.ForeColor = Color.White
        End If
        Label8.Visible = True
        Label9.Visible = True
        txtResX.Visible = True
        txtResY.Visible = True
        checkCustomRes.Visible = True
        btnSendKeys.Visible = True
        btnGenerateKeys.Visible = False
        btnUpdate.Visible = True
        txtSaveVRX.Visible = False
        btnUART2.Visible = True
        btnUART2OFF.Visible = True
        btnRestartWFB.Visible = True
        btnRestartMajestic.Visible = True
        txtSaveCam.Visible = True
        txtSaveTLM.Visible = True
        ComboBox3.Visible = True
        ComboBox4.Visible = True
        ComboBox5.Visible = True
        ComboBox6.Visible = True
        ComboBox7.Visible = True
        ComboBox8.Visible = True
        ComboBox9.Visible = True
        cmbOSD.Visible = True
        cmbFormat.Visible = True
        txtLDPC.Visible = True
        txtFECK.Visible = True
        txtFECN.Visible = True
        txtOSD.Visible = True
        txtFormat.Visible = True
        txtFreq24.Visible = True
        txtPower24.Visible = True
        txtPortVRX.Visible = True
        txtMavlinkVRX.Visible = True
        txtExtras.Visible = True
        Label2.Visible = True
        txtMCS.ReadOnly = True
        txtSTBC.ReadOnly = True
        ComboBox1.Items.Clear()
        ComboBox1.Items.Add("5180 MHz [36]")
        ComboBox1.Items.Add("5200 MHz [40]")
        ComboBox1.Items.Add("5220 MHz [44]")
        ComboBox1.Items.Add("5240 MHz [48]")
        ComboBox1.Items.Add("5260 MHz [52]")
        ComboBox1.Items.Add("5280 MHz [56]")
        ComboBox1.Items.Add("5300 MHz [60]")
        ComboBox1.Items.Add("5320 MHz [64]")
        ComboBox1.Items.Add("5500 MHz [100]")
        ComboBox1.Items.Add("5520 MHz [104]")
        ComboBox1.Items.Add("5540 MHz [108]")
        ComboBox1.Items.Add("5560 MHz [112]")
        ComboBox1.Items.Add("5580 MHz [116]")
        ComboBox1.Items.Add("5600 MHz [120]")
        ComboBox1.Items.Add("5620 MHz [124]")
        ComboBox1.Items.Add("5640 MHz [128]")
        ComboBox1.Items.Add("5660 MHz [132]")
        ComboBox1.Items.Add("5680 MHz [136]")
        ComboBox1.Items.Add("5700 MHz [140]")
        ComboBox1.Items.Add("5720 MHz [144]")
        ComboBox1.Items.Add("5745 MHz [149]")
        ComboBox1.Items.Add("5765 MHz [153]")
        ComboBox1.Items.Add("5785 MHz [157]")
        ComboBox1.Items.Add("5805 MHz [161]")
        ComboBox1.Items.Add("5825 MHz [165]")
        ComboBox1.Items.Add("5845 MHz [169]")
        ComboBox1.Items.Add("5865 MHz [173]")
        ComboBox1.Items.Add("5885 MHz [177]")
        ComboBox1.Text = "Select 5.8GHz Frequency"
        cmbResolutionVRX.Items.Clear()
        cmbResolutionVRX.Items.Add("720p60")
        cmbResolutionVRX.Items.Add("1080p60")
        cmbResolutionVRX.Items.Add("1024x768x60")
        cmbResolutionVRX.Items.Add("1366x768x60")
        cmbResolutionVRX.Items.Add("1280x1024x60")
        cmbResolutionVRX.Items.Add("1600x1200x60")
        cmbResolutionVRX.Items.Add("2560x1440x30")
        cmbResolutionVRX.Text = "Select Resolution"
        cmbCodecVRX.Items.Clear()
        cmbCodecVRX.Items.Add("h264")
        cmbCodecVRX.Items.Add("h265")
        cmbCodecVRX.Text = "Select Codec"
        txtPassword.Text = "12345"
    End Sub

    Private Sub rBtnRadxaZero3w_CheckedChanged(sender As Object, e As EventArgs) Handles rBtnRadxaZero3w.CheckedChanged
        txtIP.Text = RadxaIP
        Dim rb = DirectCast(sender, RadioButton)

        If rb.Checked Then
            rb.BackColor = Color.Gold
            rb.ForeColor = Color.Black
        Else
            rb.BackColor = Color.FromArgb(45, 45, 45)
            rb.ForeColor = Color.White
        End If
        Label8.Visible = False
        Label9.Visible = False
        txtResX.Visible = False
        txtResY.Visible = False
        checkCustomRes.Visible = False
        btnSendKeys.Visible = False
        btnGenerateKeys.Visible = True
        btnUpdate.Visible = False
        txtSaveVRX.Visible = True
        btnUART2.Visible = False
        btnUART2OFF.Visible = False
        btnRestartWFB.Visible = False
        btnRestartMajestic.Visible = False
        txtSaveCam.Visible = False
        txtSaveTLM.Visible = False
        ComboBox3.Visible = False
        ComboBox4.Visible = False
        ComboBox5.Visible = False
        ComboBox6.Visible = False
        ComboBox7.Visible = False
        ComboBox8.Visible = False
        ComboBox9.Visible = False
        cmbOSD.Visible = False
        cmbFormat.Visible = False
        txtLDPC.Visible = False
        txtFECK.Visible = False
        txtFECN.Visible = False
        txtOSD.Visible = False
        txtFormat.Visible = False
        txtFreq24.Visible = False
        txtPower24.Visible = False
        txtPortVRX.Visible = False
        txtMavlinkVRX.Visible = False
        txtExtras.Visible = False
        Label2.Visible = False
        txtMCS.ReadOnly = False
        txtSTBC.ReadOnly = False
        ComboBox1.Items.Clear()
        ComboBox1.Items.Add("2412 MHz [1]")
        ComboBox1.Items.Add("2417 MHz [2]")
        ComboBox1.Items.Add("2422 MHz [3]")
        ComboBox1.Items.Add("2427 MHz [4]")
        ComboBox1.Items.Add("2432 MHz [5]")
        ComboBox1.Items.Add("2437 MHz [6]")
        ComboBox1.Items.Add("2442 MHz [7]")
        ComboBox1.Items.Add("2447 MHz [8]")
        ComboBox1.Items.Add("2452 MHz [9]")
        ComboBox1.Items.Add("2457 MHz [10]")
        ComboBox1.Items.Add("2462 MHz [11]")
        ComboBox1.Items.Add("2467 MHz [12]")
        ComboBox1.Items.Add("2472 MHz [13]")
        ComboBox1.Items.Add("2484 MHz [14]")
        ComboBox1.Items.Add("5180 MHz [36]")
        ComboBox1.Items.Add("5200 MHz [40]")
        ComboBox1.Items.Add("5220 MHz [44]")
        ComboBox1.Items.Add("5240 MHz [48]")
        ComboBox1.Items.Add("5260 MHz [52]")
        ComboBox1.Items.Add("5280 MHz [56]")
        ComboBox1.Items.Add("5300 MHz [60]")
        ComboBox1.Items.Add("5320 MHz [64]")
        ComboBox1.Items.Add("5500 MHz [100]")
        ComboBox1.Items.Add("5520 MHz [104]")
        ComboBox1.Items.Add("5540 MHz [108]")
        ComboBox1.Items.Add("5560 MHz [112]")
        ComboBox1.Items.Add("5580 MHz [116]")
        ComboBox1.Items.Add("5600 MHz [120]")
        ComboBox1.Items.Add("5620 MHz [124]")
        ComboBox1.Items.Add("5640 MHz [128]")
        ComboBox1.Items.Add("5660 MHz [132]")
        ComboBox1.Items.Add("5680 MHz [136]")
        ComboBox1.Items.Add("5700 MHz [140]")
        ComboBox1.Items.Add("5720 MHz [144]")
        ComboBox1.Items.Add("5745 MHz [149]")
        ComboBox1.Items.Add("5765 MHz [153]")
        ComboBox1.Items.Add("5785 MHz [157]")
        ComboBox1.Items.Add("5805 MHz [161]")
        ComboBox1.Items.Add("5825 MHz [165]")
        ComboBox1.Items.Add("5845 MHz [169]")
        ComboBox1.Items.Add("5865 MHz [173]")
        ComboBox1.Items.Add("5885 MHz [177]")
        ComboBox1.Text = "Select 5.8GHz Frequency"
        cmbResolutionVRX.Items.Clear()
        cmbResolutionVRX.Items.Add("1280x720")
        cmbResolutionVRX.Items.Add("1920x1080")
        cmbResolutionVRX.Text = "Select Resolution"
        cmbCodecVRX.Items.Clear()
        cmbCodecVRX.Items.Add("20")
        cmbCodecVRX.Items.Add("30")
        cmbCodecVRX.Items.Add("50")
        cmbCodecVRX.Items.Add("60")
        cmbCodecVRX.Items.Add("90")
        cmbCodecVRX.Items.Add("100")
        cmbCodecVRX.Items.Add("110")
        cmbCodecVRX.Items.Add("120")
        cmbCodecVRX.Text = "Select FPS"
        txtPassword.Text = "root"
    End Sub

    Public Class TabControl
        Inherits System.Windows.Forms.TabControl

#Region " Windows Form Designer generated code "

        Public Sub New()
            MyBase.New()

            'This call is required by the Windows Form Designer.
            InitializeComponent()

            'Add any initialization after the InitializeComponent() call
            SetStyle(ControlStyles.AllPaintingInWmPaint Or
                ControlStyles.DoubleBuffer Or
                ControlStyles.ResizeRedraw Or
                ControlStyles.UserPaint, True)

        End Sub

        'UserControl1 overrides dispose to clean up the component list.
        Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
            If disposing Then
                If Not (components Is Nothing) Then
                    components.Dispose()
                End If
            End If
            MyBase.Dispose(disposing)
        End Sub

        'Required by the Windows Form Designer
        Private components As System.ComponentModel.IContainer

        'NOTE: The following procedure is required by the Windows Form Designer
        'It can be modified using the Windows Form Designer.  
        'Do not modify it using the code editor.
        <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
            components = New System.ComponentModel.Container
        End Sub

#End Region

#Region " InterOP "

        <StructLayout(LayoutKind.Sequential)>
        Private Structure NMHDR
            Public HWND As Int32
            Public idFrom As Int32
            Public code As Int32
            Public Overloads Function ToString() As String
                Return String.Format("Hwnd: {0}, ControlID: {1}, Code: {2}", HWND, idFrom, code)
            End Function
        End Structure

        Private Const TCN_FIRST As Int32 = &HFFFFFFFFFFFFFDDA&
        Private Const TCN_SELCHANGING As Int32 = (TCN_FIRST - 2)

        Private Const WM_USER As Int32 = &H400&
        Private Const WM_NOTIFY As Int32 = &H4E&
        Private Const WM_REFLECT As Int32 = WM_USER + &H1C00&

#End Region

#Region " BackColor Manipulation "

        'As well as exposing the property to the Designer we want it to behave just like any other 
        'controls BackColor property so we need some clever manipulation.
        Private m_Backcolor As Color = Color.Empty
        <Browsable(True),
    Description("The background color used to display text and graphics in a control.")>
        Public Overrides Property BackColor() As Color
            Get
                If m_Backcolor.Equals(Color.Empty) Then
                    If Parent Is Nothing Then
                        Return Control.DefaultBackColor
                    Else
                        Return Parent.BackColor
                    End If
                End If
                Return m_Backcolor
            End Get
            Set(ByVal Value As Color)
                If m_Backcolor.Equals(Value) Then Return
                m_Backcolor = Value
                Invalidate()
                'Let the Tabpages know that the backcolor has changed.
                MyBase.OnBackColorChanged(EventArgs.Empty)
            End Set
        End Property
        Public Function ShouldSerializeBackColor() As Boolean
            Return Not m_Backcolor.Equals(Color.Empty)
        End Function
        Public Overrides Sub ResetBackColor()
            m_Backcolor = Color.Empty
            Invalidate()
        End Sub

#End Region

        Protected Overrides Sub OnParentBackColorChanged(ByVal e As System.EventArgs)
            MyBase.OnParentBackColorChanged(e)
            Invalidate()
        End Sub

        Protected Overrides Sub OnSelectedIndexChanged(ByVal e As System.EventArgs)
            MyBase.OnSelectedIndexChanged(e)
            Invalidate()
        End Sub

        Protected Overrides Sub OnPaint(ByVal e As System.Windows.Forms.PaintEventArgs)
            MyBase.OnPaint(e)
            e.Graphics.Clear(BackColor)
            Dim r As Rectangle = Me.ClientRectangle
            If TabCount <= 0 Then Return
            'Draw a custom background for Transparent TabPages
            r = SelectedTab.Bounds
            Dim sf As New StringFormat
            sf.Alignment = StringAlignment.Center
            sf.LineAlignment = StringAlignment.Center
            Dim DrawFont As New Font(Font.FontFamily, 24, FontStyle.Regular, GraphicsUnit.Pixel)
            ControlPaint.DrawStringDisabled(e.Graphics, "Micks Ownerdraw TabControl", DrawFont, BackColor, RectangleF.op_Implicit(r), sf)
            DrawFont.Dispose()
            'Draw a border around TabPage
            r.Inflate(3, 3)
            Dim tp As TabPage = TabPages(SelectedIndex)
            Dim PaintBrush As New SolidBrush(tp.BackColor)
            e.Graphics.FillRectangle(PaintBrush, r)
            ControlPaint.DrawBorder(e.Graphics, r, PaintBrush.Color, ButtonBorderStyle.Outset)
            'Draw the Tabs
            For index As Integer = 0 To TabCount - 1
                tp = TabPages(index)
                r = GetTabRect(index)
                Dim bs As ButtonBorderStyle = ButtonBorderStyle.Outset
                If index = SelectedIndex Then bs = ButtonBorderStyle.Inset
                PaintBrush.Color = Color.Gold
                e.Graphics.FillRectangle(PaintBrush, r)
                ControlPaint.DrawBorder(e.Graphics, r, PaintBrush.Color, bs)
                PaintBrush.Color = Color.Black

                'Set up rotation for left and right aligned tabs
                If Alignment = TabAlignment.Left Or Alignment = TabAlignment.Right Then
                    Dim RotateAngle As Single = 90
                    If Alignment = TabAlignment.Left Then RotateAngle = 270
                    Dim cp As New PointF(r.Left + (r.Width \ 2), r.Top + (r.Height \ 2))
                    e.Graphics.TranslateTransform(cp.X, cp.Y)
                    e.Graphics.RotateTransform(RotateAngle)
                    r = New Rectangle(-(r.Height \ 2), -(r.Width \ 2), r.Height, r.Width)
                End If
                'Draw the Tab Text
                If tp.Enabled Then
                    e.Graphics.DrawString(tp.Text, Font, PaintBrush, RectangleF.op_Implicit(r), sf)
                Else
                    ControlPaint.DrawStringDisabled(e.Graphics, tp.Text, Font, tp.BackColor, RectangleF.op_Implicit(r), sf)
                End If

                e.Graphics.ResetTransform()

            Next
            PaintBrush.Dispose()
        End Sub

        <Description("Occurs as a tab is being changed.")>
        Public Event SelectedIndexChanging As SelectedTabPageChangeEventHandler

        Protected Overrides Sub WndProc(ByRef m As System.Windows.Forms.Message)
            If m.Msg = (WM_REFLECT + WM_NOTIFY) Then
                Dim hdr As NMHDR = DirectCast(Marshal.PtrToStructure(m.LParam, GetType(NMHDR)), NMHDR)
                If hdr.code = TCN_SELCHANGING Then
                    Dim tp As TabPage = TestTab(Me.PointToClient(Cursor.Position))
                    If Not tp Is Nothing Then
                        Dim e As New TabPageChangeEventArgs(Me.SelectedTab, tp)
                        RaiseEvent SelectedIndexChanging(Me, e)
                        If e.Cancel OrElse tp.Enabled = False Then
                            m.Result = New IntPtr(1)
                            Return
                        End If
                    End If
                End If
            End If
            MyBase.WndProc(m)
        End Sub

        Private Function TestTab(ByVal pt As Point) As TabPage
            For index As Integer = 0 To TabCount - 1
                If GetTabRect(index).Contains(pt.X, pt.Y) Then
                    Return TabPages(index)
                End If
            Next
            Return Nothing
        End Function

    End Class

#Region " EventArgs Class's "

    Public Class TabPageChangeEventArgs
        Inherits EventArgs

        Private _Selected As TabPage
        Private _PreSelected As TabPage
        Public Cancel As Boolean = False

        Public ReadOnly Property CurrentTab() As TabPage
            Get
                Return _Selected
            End Get
        End Property

        Public ReadOnly Property NextTab() As TabPage
            Get
                Return _PreSelected
            End Get
        End Property

        Public Sub New(ByVal CurrentTab As TabPage, ByVal NextTab As TabPage)
            _Selected = CurrentTab
            _PreSelected = NextTab
        End Sub

    End Class

    Public Delegate Sub SelectedTabPageChangeEventHandler(ByVal sender As Object, ByVal e As TabPageChangeEventArgs)

    Private Sub btnUART2_Click_1(sender As Object, e As EventArgs) Handles btnUART2.Click
        Dim extern = "extern.bat"
        If Not IO.File.Exists(extern) Then
            MsgBox("File " + extern + " not found!")
            Return
        End If

        If IsValidIP(txtIP.Text) Then
            With New Process()
                .StartInfo.UseShellExecute = False
                .StartInfo.FileName = extern
                .StartInfo.Arguments = "UART2 " + String.Format("{0}", txtIP.Text) + " " + txtPassword.Text
                .StartInfo.RedirectStandardOutput = False
                .Start()
            End With
        Else
            MsgBox("Please enter a valid IP address")
        End If
    End Sub

    Private Sub btnUART2OFF_Click_1(sender As Object, e As EventArgs) Handles btnUART2OFF.Click
        Dim extern = "extern.bat"
        If Not IO.File.Exists(extern) Then
            MsgBox("File " + extern + " not found!")
            Return
        End If

        If IsValidIP(txtIP.Text) Then
            With New Process()
                .StartInfo.UseShellExecute = False
                .StartInfo.FileName = extern
                .StartInfo.Arguments = "UART0 " + String.Format("{0}", txtIP.Text) + " " + txtPassword.Text
                .StartInfo.RedirectStandardOutput = False
                .Start()
            End With
        Else
            MsgBox("Please enter a valid IP address")
        End If
    End Sub

    Private Sub btnLEFT_Click(sender As Object, e As EventArgs) Handles btnLEFT.Click
        If RadioButton1.Checked Then RadioButton1.Left = RadioButton1.Left - 2
        If RadioButton2.Checked Then RadioButton2.Left = RadioButton2.Left - 2
        If RadioButton3.Checked Then RadioButton3.Left = RadioButton3.Left - 2
        If RadioButton4.Checked Then RadioButton4.Left = RadioButton4.Left - 2
        If RadioButton5.Checked Then RadioButton5.Left = RadioButton5.Left - 2
        If RadioButton6.Checked Then RadioButton6.Left = RadioButton6.Left - 2
        If RadioButton7.Checked Then RadioButton7.Left = RadioButton7.Left - 2
        If RadioButton8.Checked Then RadioButton8.Left = RadioButton8.Left - 2
        If RadioButton9.Checked Then RadioButton9.Left = RadioButton9.Left - 2
        If RadioButton10.Checked Then RadioButton10.Left = RadioButton10.Left - 2
        If RadioButton11.Checked Then RadioButton11.Left = RadioButton11.Left - 2
        If RadioButton12.Checked Then RadioButton12.Left = RadioButton12.Left - 2
        If RadioButton13.Checked Then RadioButton13.Left = RadioButton13.Left - 2
        If RadioButton14.Checked Then RadioButton14.Left = RadioButton14.Left - 2
        If RadioButton15.Checked Then RadioButton15.Left = RadioButton15.Left - 2
        If RadioButton16.Checked Then RadioButton16.Left = RadioButton16.Left - 2
        If RadioButton17.Checked Then RadioButton17.Left = RadioButton17.Left - 2
    End Sub

    Private Sub btnRIGHT_Click(sender As Object, e As EventArgs) Handles btnRIGHT.Click
        If RadioButton1.Checked Then RadioButton1.Left = RadioButton1.Left + 2
        If RadioButton2.Checked Then RadioButton2.Left = RadioButton2.Left + 2
        If RadioButton3.Checked Then RadioButton3.Left = RadioButton3.Left + 2
        If RadioButton4.Checked Then RadioButton4.Left = RadioButton4.Left + 2
        If RadioButton5.Checked Then RadioButton5.Left = RadioButton5.Left + 2
        If RadioButton6.Checked Then RadioButton6.Left = RadioButton6.Left + 2
        If RadioButton7.Checked Then RadioButton7.Left = RadioButton7.Left + 2
        If RadioButton8.Checked Then RadioButton8.Left = RadioButton8.Left + 2
        If RadioButton9.Checked Then RadioButton9.Left = RadioButton9.Left + 2
        If RadioButton10.Checked Then RadioButton10.Left = RadioButton10.Left + 2
        If RadioButton11.Checked Then RadioButton11.Left = RadioButton11.Left + 2
        If RadioButton12.Checked Then RadioButton12.Left = RadioButton12.Left + 2
        If RadioButton13.Checked Then RadioButton13.Left = RadioButton13.Left + 2
        If RadioButton14.Checked Then RadioButton14.Left = RadioButton14.Left + 2
        If RadioButton15.Checked Then RadioButton15.Left = RadioButton15.Left + 2
        If RadioButton16.Checked Then RadioButton16.Left = RadioButton16.Left + 2
        If RadioButton17.Checked Then RadioButton17.Left = RadioButton17.Left + 2
    End Sub

    Private Sub btnUP_Click(sender As Object, e As EventArgs) Handles btnUP.Click
        If RadioButton1.Checked Then RadioButton1.Top = RadioButton1.Top - 2
        If RadioButton2.Checked Then RadioButton2.Top = RadioButton2.Top - 2
        If RadioButton3.Checked Then RadioButton3.Top = RadioButton3.Top - 2
        If RadioButton4.Checked Then RadioButton4.Top = RadioButton4.Top - 2
        If RadioButton5.Checked Then RadioButton5.Top = RadioButton5.Top - 2
        If RadioButton6.Checked Then RadioButton6.Top = RadioButton6.Top - 2
        If RadioButton7.Checked Then RadioButton7.Top = RadioButton7.Top - 2
        If RadioButton8.Checked Then RadioButton8.Top = RadioButton8.Top - 2
        If RadioButton9.Checked Then RadioButton9.Top = RadioButton9.Top - 2
        If RadioButton10.Checked Then RadioButton10.Top = RadioButton10.Top - 2
        If RadioButton11.Checked Then RadioButton11.Top = RadioButton11.Top - 2
        If RadioButton12.Checked Then RadioButton12.Top = RadioButton12.Top - 2
        If RadioButton13.Checked Then RadioButton13.Top = RadioButton13.Top - 2
        If RadioButton14.Checked Then RadioButton14.Top = RadioButton14.Top - 2
        If RadioButton15.Checked Then RadioButton15.Top = RadioButton15.Top - 2
        If RadioButton16.Checked Then RadioButton16.Top = RadioButton16.Top - 2
        If RadioButton17.Checked Then RadioButton17.Top = RadioButton17.Top - 2
        If RadioButton18.Checked Then
            RadioButton18.Left = RadioButton18.Left - 2
            RadioButton18.Top = RadioButton18.Top - 2
        End If
    End Sub

    Private Sub btnDOWN_Click(sender As Object, e As EventArgs) Handles btnDOWN.Click
        If RadioButton1.Checked Then RadioButton1.Top = RadioButton1.Top + 2
        If RadioButton2.Checked Then RadioButton2.Top = RadioButton2.Top + 2
        If RadioButton3.Checked Then RadioButton3.Top = RadioButton3.Top + 2
        If RadioButton4.Checked Then RadioButton4.Top = RadioButton4.Top + 2
        If RadioButton5.Checked Then RadioButton5.Top = RadioButton5.Top + 2
        If RadioButton6.Checked Then RadioButton6.Top = RadioButton6.Top + 2
        If RadioButton7.Checked Then RadioButton7.Top = RadioButton7.Top + 2
        If RadioButton8.Checked Then RadioButton8.Top = RadioButton8.Top + 2
        If RadioButton9.Checked Then RadioButton9.Top = RadioButton9.Top + 2
        If RadioButton10.Checked Then RadioButton10.Top = RadioButton10.Top + 2
        If RadioButton11.Checked Then RadioButton11.Top = RadioButton11.Top + 2
        If RadioButton12.Checked Then RadioButton12.Top = RadioButton12.Top + 2
        If RadioButton13.Checked Then RadioButton13.Top = RadioButton13.Top + 2
        If RadioButton14.Checked Then RadioButton14.Top = RadioButton14.Top + 2
        If RadioButton15.Checked Then RadioButton15.Top = RadioButton15.Top + 2
        If RadioButton16.Checked Then RadioButton16.Top = RadioButton16.Top + 2
        If RadioButton17.Checked Then RadioButton17.Top = RadioButton17.Top + 2
        If RadioButton18.Checked Then
            RadioButton18.Left = RadioButton18.Left + 2
            RadioButton18.Top = RadioButton18.Top + 2
        End If
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim vdec = "vdec.conf"
        If Not IO.File.Exists(vdec) Then
            MsgBox("File " + vdec + " not found!")
            Return
        End If

        If CheckBox1.Checked = False Then RadioButton1.Left = 0
        If CheckBox2.Checked = False Then RadioButton2.Left = 0
        If CheckBox3.Checked = False Then RadioButton3.Left = 0
        If CheckBox4.Checked = False Then RadioButton4.Left = 0
        If CheckBox5.Checked = False Then RadioButton5.Left = 0
        If CheckBox6.Checked = False Then RadioButton6.Left = 0
        If CheckBox7.Checked = False Then RadioButton7.Left = 0
        If CheckBox8.Checked = False Then RadioButton8.Left = 0
        If CheckBox9.Checked = False Then RadioButton9.Left = 0
        If CheckBox10.Checked = False Then RadioButton10.Left = 0
        If CheckBox11.Checked = False Then RadioButton11.Left = 0
        If CheckBox12.Checked = False Then RadioButton12.Left = 0
        If CheckBox13.Checked = False Then RadioButton13.Left = 0
        If CheckBox14.Checked = False Then RadioButton14.Left = 0
        If CheckBox15.Checked = False Then RadioButton15.Left = 0
        If CheckBox16.Checked = False Then RadioButton16.Left = 0
        If CheckBox17.Checked = False Then RadioButton17.Left = 0
        If CheckBox18.Checked = False Then RadioButton18.Left = 0

        Dim VDECfilePath = vdec
        Dim lines = IO.File.ReadAllLines(VDECfilePath)
        For y = 0 To lines.Count - 1
            If lines(y).StartsWith("osd_elements=") Then
                lines(y) = "osd_elements=""-osd_ele1x " + CStr(RadioButton1.Left * 2.5) + " -osd_ele1y " + CStr(RadioButton1.Top * 2.5) +
                                         " -osd_ele2x " + CStr(RadioButton2.Left * 2.5) + " -osd_ele2y " + CStr(RadioButton2.Top * 2.5) +
                                         " -osd_ele3x " + CStr(RadioButton3.Left * 2.5) + " -osd_ele3y " + CStr(RadioButton3.Top * 2.5) +
                                         " -osd_ele4x " + CStr(RadioButton4.Left * 2.5) + " -osd_ele4y " + CStr(RadioButton4.Top * 2.5) +
                                         " -osd_ele5x " + CStr(RadioButton5.Left * 2.5) + " -osd_ele5y " + CStr(RadioButton5.Top * 2.5) +
                                         " -osd_ele6x " + CStr(RadioButton6.Left * 2.5) + " -osd_ele6y " + CStr(RadioButton6.Top * 2.5) +
                                         " -osd_ele7x " + CStr(RadioButton7.Left * 2.5) + " -osd_ele7y " + CStr(RadioButton7.Top * 2.5) +
                                         " -osd_ele8x " + CStr(RadioButton8.Left * 2.5) + " -osd_ele8y " + CStr(RadioButton8.Top * 2.5) +
                                         " -osd_ele9x " + CStr(RadioButton9.Left * 2.5) + " -osd_ele9y " + CStr(RadioButton9.Top * 2.5) +
                                         " -osd_ele10x " + CStr(RadioButton10.Left * 2.5) + " -osd_ele10y " + CStr(RadioButton10.Top * 2.5) +
                                         " -osd_ele11x " + CStr(RadioButton11.Left * 2.5) + " -osd_ele11y " + CStr(RadioButton11.Top * 2.5) +
                                         " -osd_ele12x " + CStr(RadioButton12.Left * 2.5) + " -osd_ele12y " + CStr(RadioButton12.Top * 2.5) +
                                         " -osd_ele13x " + CStr(RadioButton13.Left * 2.5) + " -osd_ele13y " + CStr(RadioButton13.Top * 2.5) +
                                         " -osd_ele14x " + CStr(RadioButton14.Left * 2.5) + " -osd_ele14y " + CStr(RadioButton14.Top * 2.5) +
                                         " -osd_ele15x " + CStr(RadioButton15.Left * 2.5) + " -osd_ele15y " + CStr(RadioButton15.Top * 2.5) +
                                         " -osd_ele16x " + CStr(RadioButton16.Left * 2.5) + " -osd_ele16y " + CStr(RadioButton16.Top * 2.5) +
                                         " -osd_ele17x " + CStr(RadioButton17.Left * 2.5) + " -osd_ele17y " + CStr(RadioButton17.Top * 2.5) +
                                         " -osd_ele18x " + CStr((RadioButton18.Left - 128) * 2.5) + " -osd_ele18y " + CStr((RadioButton18.Top - 124) * 2.5) + """"
            End If
        Next
        IO.File.WriteAllLines(VDECfilePath, lines)
        MsgBox("Settings saved successfully", MsgBoxStyle.Information, "OpenIPC")
    End Sub

    Private Sub btnScan_Click(sender As Object, e As EventArgs) Handles btnScan.Click
        lblScan.Text = "Available IP Addresses on your network:"
        BackgroundWorker1.RunWorkerAsync()
    End Sub

    Private Sub BackgroundWorker1_DoWork(sender As Object, e As System.ComponentModel.DoWorkEventArgs) Handles BackgroundWorker1.DoWork
        Dim ping As Ping
        Dim pingReply As PingReply

        Thread.Sleep(500)

        Parallel.For(0, 254, Sub(i, loopState)
                                 ping = New Ping()
                                 pingReply = ping.Send(txtScan.Text + i.ToString())
                                 Me.BeginInvoke(CType(Sub()
                                                          If pingReply.Status = IPStatus.Success Then
                                                              lblScan.Text = lblScan.Text + vbCrLf + txtScan.Text + i.ToString()
                                                          End If
                                                      End Sub, Action))
                             End Sub)
        MessageBox.Show("Scan completed")
    End Sub

    Private Sub CheckBox1_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox1.CheckedChanged
        If CheckBox1.Checked = False Then
            RadioButton1.Visible = False
        Else
            RadioButton1.Visible = True
        End If
    End Sub

    Private Sub CheckBox2_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox2.CheckedChanged
        If CheckBox2.Checked = False Then
            RadioButton2.Visible = False
        Else
            RadioButton2.Visible = True
        End If
    End Sub

    Private Sub CheckBox3_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox3.CheckedChanged
        If CheckBox3.Checked = False Then
            RadioButton3.Visible = False
        Else
            RadioButton3.Visible = True
        End If
    End Sub

    Private Sub CheckBox4_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox4.CheckedChanged
        If CheckBox4.Checked = False Then
            RadioButton4.Visible = False
        Else
            RadioButton4.Visible = True
        End If
    End Sub

    Private Sub CheckBox5_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox5.CheckedChanged
        If CheckBox5.Checked = False Then
            RadioButton5.Visible = False
        Else
            RadioButton5.Visible = True
        End If
    End Sub

    Private Sub CheckBox6_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox6.CheckedChanged
        If CheckBox6.Checked = False Then
            RadioButton6.Visible = False
        Else
            RadioButton6.Visible = True
        End If
    End Sub

    Private Sub CheckBox7_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox7.CheckedChanged
        If CheckBox7.Checked = False Then
            RadioButton7.Visible = False
        Else
            RadioButton7.Visible = True
        End If
    End Sub

    Private Sub CheckBox8_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox8.CheckedChanged
        If CheckBox8.Checked = False Then
            RadioButton8.Visible = False
        Else
            RadioButton8.Visible = True
        End If
    End Sub

    Private Sub CheckBox9_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox9.CheckedChanged
        If CheckBox9.Checked = False Then
            RadioButton9.Visible = False
        Else
            RadioButton9.Visible = True
        End If
    End Sub

    Private Sub CheckBox10_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox10.CheckedChanged
        If CheckBox10.Checked = False Then
            RadioButton10.Visible = False
        Else
            RadioButton10.Visible = True
        End If
    End Sub

    Private Sub CheckBox11_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox11.CheckedChanged
        If CheckBox11.Checked = False Then
            RadioButton11.Visible = False
        Else
            RadioButton11.Visible = True
        End If
    End Sub

    Private Sub CheckBox12_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox12.CheckedChanged
        If CheckBox12.Checked = False Then
            RadioButton12.Visible = False
        Else
            RadioButton12.Visible = True
        End If
    End Sub

    Private Sub CheckBox13_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox13.CheckedChanged
        If CheckBox13.Checked = False Then
            RadioButton13.Visible = False
        Else
            RadioButton13.Visible = True
        End If
    End Sub

    Private Sub CheckBox14_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox14.CheckedChanged
        If CheckBox14.Checked = False Then
            RadioButton14.Visible = False
        Else
            RadioButton14.Visible = True
        End If
    End Sub

    Private Sub CheckBox15_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox15.CheckedChanged
        If CheckBox15.Checked = False Then
            RadioButton15.Visible = False
        Else
            RadioButton15.Visible = True
        End If
    End Sub

    Private Sub CheckBox16_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox16.CheckedChanged
        If CheckBox16.Checked = False Then
            RadioButton16.Visible = False
        Else
            RadioButton16.Visible = True
        End If
    End Sub

    Private Sub CheckBox17_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox17.CheckedChanged
        If CheckBox17.Checked = False Then
            RadioButton17.Visible = False
        Else
            RadioButton17.Visible = True
        End If
    End Sub

    Private Sub CheckBox18_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox18.CheckedChanged
        If CheckBox18.Checked = False Then
            RadioButton18.Visible = False
        Else
            RadioButton18.Visible = True
        End If
    End Sub

    Private Sub btnSensor_Click(sender As Object, e As EventArgs) Handles btnSensor.Click
        Dim extern = "extern.bat"
        If Not IO.File.Exists(extern) Then
            MsgBox("File " + extern + " not found!")
            Return
        End If

        If IsValidIP(txtIP.Text) Then
            With New Process()
                .StartInfo.UseShellExecute = False
                .StartInfo.FileName = extern
                .StartInfo.Arguments = "binup " + String.Format("{0}", txtIP.Text) + " " + txtPassword.Text + " " + txtBin.Text
                .StartInfo.RedirectStandardOutput = False
                .Start()
            End With
        Else
            MsgBox("Please enter a valid IP address")
        End If
    End Sub

    Private Sub btnDriver_Click(sender As Object, e As EventArgs) Handles btnDriver.Click
        Dim extern = "extern.bat"
        If Not IO.File.Exists(extern) Then
            MsgBox("File " + extern + " not found!")
            Return
        End If

        If IsValidIP(txtIP.Text) Then
            With New Process()
                .StartInfo.UseShellExecute = False
                .StartInfo.FileName = extern
                .StartInfo.Arguments = "koup " + String.Format("{0}", txtIP.Text) + " " + txtPassword.Text + " " + txtDriver.Text
                .StartInfo.RedirectStandardOutput = False
                .Start()
            End With
        Else
            MsgBox("Please enter a valid IP address")
        End If
    End Sub

    Private Sub btnBinBackup_Click(sender As Object, e As EventArgs) Handles btnBinBackup.Click
        Dim extern = "extern.bat"
        If Not IO.File.Exists(extern) Then
            MsgBox("File " + extern + " not found!")
            Return
        End If

        If IsValidIP(txtIP.Text) Then
            With New Process()
                .StartInfo.UseShellExecute = False
                .StartInfo.FileName = extern
                .StartInfo.Arguments = "bindl " + String.Format("{0}", txtIP.Text) + " " + txtPassword.Text + " " + txtBin.Text
                .StartInfo.RedirectStandardOutput = False
                .Start()
            End With
        Else
            MsgBox("Please enter a valid IP address")
        End If
    End Sub

    Private Sub btnDriverBackup_Click(sender As Object, e As EventArgs) Handles btnDriverBackup.Click
        Dim extern = "extern.bat"
        If Not IO.File.Exists(extern) Then
            MsgBox("File " + extern + " not found!")
            Return
        End If

        If IsValidIP(txtIP.Text) Then
            With New Process()
                .StartInfo.UseShellExecute = False
                .StartInfo.FileName = extern
                .StartInfo.Arguments = "kodl " + String.Format("{0}", txtIP.Text) + " " + txtPassword.Text + " " + txtDriver.Text
                .StartInfo.RedirectStandardOutput = False
                .Start()
            End With
        Else
            MsgBox("Please enter a valid IP address")
        End If
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Dim extern = "extern.bat"
        If Not IO.File.Exists(extern) Then
            MsgBox("File " + extern + " not found!")
            Return
        End If

        If IsValidIP(txtIP.Text) Then
            With New Process()
                .StartInfo.UseShellExecute = False
                .StartInfo.FileName = extern
                .StartInfo.Arguments = "shdl " + String.Format("{0}", txtIP.Text) + " " + txtPassword.Text
                .StartInfo.RedirectStandardOutput = False
                .Start()
            End With
        Else
            MsgBox("Please enter a valid IP address")
        End If
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        Dim extern = "extern.bat"
        If Not IO.File.Exists(extern) Then
            MsgBox("File " + extern + " not found!")
            Return
        End If

        If IsValidIP(txtIP.Text) Then
            With New Process()
                .StartInfo.UseShellExecute = False
                .StartInfo.FileName = extern
                .StartInfo.Arguments = "shup " + String.Format("{0}", txtIP.Text) + " " + txtPassword.Text + " " + txtBin.Text
                .StartInfo.RedirectStandardOutput = False
                .Start()
            End With
        Else
            MsgBox("Please enter a valid IP address")
        End If
    End Sub

    Private Sub checkCustomRes_CheckedChanged(sender As Object, e As EventArgs) Handles checkCustomRes.CheckedChanged
        If checkCustomRes.Checked = True Then
            txtExtras.Text = "extra=""--bg-r 0 --bg-g 0 --bg-b 50 --ar manual --ar-w " + txtResX.Text + " --ar-h " + txtResX.Text + """"
        Else
            txtExtras.Text = "extra=""--bg-r 0 --bg-g 0 --bg-b 50"""
        End If
    End Sub

    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        Dim extern = "extern.bat"
        If Not File.Exists(extern) Then
            MsgBox("File " + extern + " not found!")
            Return
        End If

        If IsValidIP(txtIP.Text) Then
            With New Process()
                .StartInfo.UseShellExecute = False
                .StartInfo.FileName = extern
                .StartInfo.Arguments = "temp " + String.Format("{0}", txtIP.Text) + " " + txtPassword.Text
                .StartInfo.RedirectStandardOutput = False
                .Start()
            End With
        Else
            MsgBox("Please enter a valid IP address")
        End If
    End Sub

#End Region
End Class
