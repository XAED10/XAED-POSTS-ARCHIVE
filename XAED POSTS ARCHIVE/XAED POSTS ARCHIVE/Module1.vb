Imports System.IO, System.Net, System.Text, System.Text.RegularExpressions, System.Console
Imports System.Threading
Imports Microsoft.VisualBasic.CompilerServices
Imports RestSharp
Module Module1
    Dim cookies As New CookieContainer
    Dim Process As Boolean
    Dim Error_attempts As Integer
    Dim Successful_attempts As Integer
    Dim username, password As String
    Dim myid As String
    Dim codetype As String
    Dim banner As String = "  _____   ____   _____ _______ _____            _____   _____ _    _ _______      ________ 
 |  __ \ / __ \ / ____|__   __/ ____|     /\   |  __ \ / ____| |  | |_   _\ \    / /  ____|
 | |__) | |  | | (___    | | | (___      /  \  | |__) | |    | |__| | | |  \ \  / /| |__   
 |  ___/| |  | |\___ \   | |  \___ \    / /\ \ |  _  /| |    |  __  | | |   \ \/ / |  __|  
 | |    | |__| |____) |  | |  ____) |  / ____ \| | \ \| |____| |  | |_| |_   \  /  | |____ 
 |_|     \____/|_____/   |_| |_____/  /_/    \_\_|  \_\\_____|_|  |_|_____|   \/   |______|
                                                                                           "
    Sub Main()
again:
        Title = "XAED POSTS ARCHIVE"
        ForegroundColor = ConsoleColor.Blue
        Write(banner)
        ForegroundColor = ConsoleColor.Magenta
        Write(vbNewLine + "[+] Username : ")
        username = ReadLine()
        Write("[+] Password : ")
        password = ReadLine()
        If Login(username, password) Then
            ForegroundColor = ConsoleColor.Green
            Write("[+] Logged In Successfully")
            Thread.Sleep(1000)
            GoTo continue1
        Else
            ForegroundColor = ConsoleColor.Red
            Write("Wrong Information Press Enter To Try Again")
            ReadLine()
            Console.Clear()
            GoTo again
        End If
continue1:
        Write(vbNewLine & vbNewLine & "[+] Started" & vbNewLine)
        start()
    End Sub
    Sub start()
        While Process = True
            GetID_FormPost(myid)
            Title = "XAED POSTS ARCHIVE | Archived Posts : " & Successful_attempts & " | Errors : " & Error_attempts
        End While
    End Sub
    Function Login(username As String, password As String) As Boolean
        Dim guidString As String = Guid.NewGuid().ToString()
        Try
            Dim data As Byte() = Encoding.UTF8.GetBytes($"username={username}&password={password}&device_id={Guid.NewGuid}&login_attempt_count=0")
            Dim req As HttpWebRequest = DirectCast(WebRequest.Create("https://i.instagram.com/api/v1/accounts/login/"), HttpWebRequest)
            With req
                .Method = "POST"
                .CookieContainer = cookies
                .Proxy = Nothing
                .UserAgent = "Instagram 100.1.0.29.135 Android (25/7.1.2; 192dpi; 720x1280; google; G011A; G011A; qcom; en_US; 262886984)"
                .ContentType = ("application/x-www-form-urlencoded")
            End With
            Dim stream As Stream = req.GetRequestStream : stream.Write(data, 0, data.Length) : stream.Dispose() : stream.Close()
            Dim httpresponse As HttpWebResponse = req.GetResponse : Dim reader As New StreamReader(httpresponse.GetResponseStream) : Dim response As String = DirectCast(reader.ReadToEnd, String) : reader.Dispose() : reader.Close()
            If response.Contains("logged_in_user") Then
                Dim getdata As String = httpresponse.GetResponseHeader("set-cookie")
                myid = Regex.Match(response, """pk"":(.*?),").Groups(1).Value ': MsgBox(myid)
                Process = True
                Return True
            ElseIf response.Contains("challenge_required") Then
                Dim url As String = Regex.Match(response, """api_path"":""(.*?)""").Groups(1).Value
                Write("[0] Phone Number | [1] Email" & vbNewLine & "[?] : ")
                Dim choice1 As String = ReadLine()
                If SendEmail(url, choice1) Then
                    Return True
                End If
            Else
                MsgBox("Bad Username or Password !", MsgBoxStyle.Critical)
                ProjectData.EndApp()
                MsgBox("Bad Username or Password !", MsgBoxStyle.Critical)
            End If
        Catch ex As WebException
            Dim exx As String = New IO.StreamReader(ex.Response.GetResponseStream()).ReadToEnd()
            If exx.Contains("Incorrect") Or exx.Contains("bad") Or exx.Contains("Bad") Then
                MsgBox("Bad Username or Password !", MsgBoxStyle.Critical)
                ProjectData.EndApp()
                MsgBox("Bad Username or Password !", MsgBoxStyle.Critical)
            ElseIf exx.Contains("challenge_required") Then
                Dim url As String = Regex.Match(exx, """api_path"":""(.*?)""").Groups(1).Value
                Write("[0] Phone Number | [1] Email" & vbNewLine & "[?] : ")
                Dim choice1 As String = ReadLine()
                If SendEmail(url, choice1) Then
                    Return True
                End If
            Else
                    MsgBox(exx)
                Interaction.MsgBox("Error on login !", MsgBoxStyle.Critical, Nothing)
                ProjectData.EndApp()
                Interaction.MsgBox("Error on login !", MsgBoxStyle.Critical, Nothing)
            End If
        End Try
    End Function
    Function SendEmail(url As String, choice As String) As Boolean
        Try
            Dim data As Byte() = Encoding.UTF8.GetBytes($"choice={choice}")
            Dim req As HttpWebRequest = DirectCast(WebRequest.Create("https://i.instagram.com/api/v1" & url), HttpWebRequest)
            With req
                .Method = "POST"
                .CookieContainer = cookies
                .Proxy = Nothing
                .UserAgent = "Instagram 100.1.0.29.135 Android (25/7.1.2; 192dpi; 720x1280; google; G011A; G011A; qcom; en_US; 262886984)"
                .ContentType = ("application/x-www-form-urlencoded")
            End With
            Dim stream As Stream = req.GetRequestStream : stream.Write(data, 0, data.Length) : stream.Dispose() : stream.Close()
            Dim httpresponse As HttpWebResponse = DirectCast(req.GetResponse, HttpWebResponse) : Dim reader As New StreamReader(httpresponse.GetResponseStream) : Dim response As String = reader.ReadToEnd
            If response.Contains("security_code") Then
                Dim code As String
                Write("[?] Code : ")
                code = ReadLine()
                If Security_code(url, code) Then
                    Return True
                End If
            Else
                MsgBox("error on sending the code", MsgBoxStyle.Critical)
                ProjectData.EndApp()
                MsgBox("error on sending the code", MsgBoxStyle.Critical)
            End If
        Catch ex As WebException
            MsgBox("error on sending the code", MsgBoxStyle.Critical)
            ProjectData.EndApp()
            MsgBox("error on sending the code", MsgBoxStyle.Critical)
        End Try
    End Function
    Function Security_code(url As String, Codetext As String) As Boolean
        Try
            Dim data As Byte() = Encoding.UTF8.GetBytes("security_code=" & Codetext)
            Dim req As HttpWebRequest = DirectCast(WebRequest.Create("https://i.instagram.com/api/v1" & url), HttpWebRequest)
            With req
                .Method = "POST"
                .CookieContainer = cookies
                .Proxy = Nothing
                .UserAgent = "Instagram 100.1.0.29.135 Android (25/7.1.2; 192dpi; 720x1280; google; G011A; G011A; qcom; en_US; 262886984)"
                .ContentType = ("application/x-www-form-urlencoded")
            End With
            Dim stream As Stream = req.GetRequestStream : stream.Write(data, 0, data.Length) : stream.Dispose() : stream.Close()
            Dim httpresponse As HttpWebResponse = DirectCast(req.GetResponse, HttpWebResponse) : Dim reader As New StreamReader(httpresponse.GetResponseStream) : Dim response As String = reader.ReadToEnd
            If response.Contains("logged_in_user") Then
                Dim getdata As String = httpresponse.GetResponseHeader("set-cookie")
                myid = Regex.Match(response, """pk"":(.*?),").Groups(1).Value ': MsgBox(myid)
                Process = True
                Return True
            Else
                Interaction.MsgBox("Error on login !", MsgBoxStyle.Critical, Nothing)
                ProjectData.EndApp()
                Interaction.MsgBox("Error on login !", MsgBoxStyle.Critical, Nothing)
            End If
        Catch ex As WebException
            Dim exresponse As String = New StreamReader(ex.Response.GetResponseStream).ReadToEnd
            Interaction.MsgBox("Error on login !", MsgBoxStyle.Critical, Nothing)
            ProjectData.EndApp()
            Interaction.MsgBox("Error on login !", MsgBoxStyle.Critical, Nothing)
        End Try
    End Function
    Function GetID_FormPost(usernameID As String) As String
        Dim RestClient As New RestClient("https://i.instagram.com/api/v1/")
        Dim RestRequest As New RestRequest($"feed/user/{usernameID}/?exclude_comment=true&only_fetch_first_carousel_media=false", Method.GET)
        RestClient.UserAgent = "Instagram 100.1.0.29.135 Android (25/7.1.2; 192dpi; 720x1280; google; G011A; G011A; qcom; en_US; 262886984)"
        RestClient.AddDefaultHeader("Content-Type", "application/x-www-form-urlencoded")
        RestClient.CookieContainer = cookies
        Dim Response As String = RestClient.Execute(RestRequest).Content
        If Response.Contains("id") Then
            Dim PostID As String = Regex.Match(Response, """id"":""(.*?)""").Groups(1).Value
            Thread.Sleep(7000)
            Return Archive_Post(PostID)
        ElseIf Response.Contains("wait") Then
            Error_attempts += 1
        End If
        If Not Response.Contains("id") Then
            Process = False
            Write(vbNewLine & vbNewLine & "[+] All posts Are Archived !")
            Thread.Sleep(1500)
            ForegroundColor = ConsoleColor.Red
            Write(vbNewLine & vbNewLine & "[X] Press Enter To Exit")
            ReadLine()
            Environment.Exit(0)
        End If
        Return Response
    End Function
    Function Archive_Post(PostID As String) As String
        Dim RestClient As New RestClient("https://i.instagram.com/api/v1/")
        Dim RestRequest As New RestRequest($"media/{PostID}/only_me/", Method.POST)
        RestClient.UserAgent = "Instagram 100.1.0.29.135 Android (25/7.1.2; 192dpi; 720x1280; google; G011A; G011A; qcom; en_US; 262886984)"
        RestClient.AddDefaultHeader("Content-Type", "application/x-www-form-urlencoded")
        RestClient.CookieContainer = cookies
        RestRequest.AddParameter("", $"media_id={PostID}&_csrftoken=missing&_uid={myid}&_uuid={Guid.NewGuid}", ParameterType.RequestBody)
        Dim Response As String = RestClient.Execute(RestRequest).Content
        If Response.Contains("""status"":""ok""") Then
            Successful_attempts += 1
            Write(vbNewLine & $"[+] Archived [{Successful_attempts} Post] [Post Id : {PostID}]")
        ElseIf Response.Contains("wait") Then
            Error_attempts += 1
        End If
        Return Response
    End Function
End Module
