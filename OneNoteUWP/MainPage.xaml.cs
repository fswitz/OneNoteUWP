using Microsoft.Identity.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SQLite;
using Syncfusion.Data;
using Syncfusion.UI.Xaml.Grid;
using Syncfusion.UI.Xaml.Grid.ScrollAxis;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace OneNoteUWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private string graphAPIEndpoint = "https://graph.microsoft.com/v1.0/me/onenote/pages";
        private string[] scopes = { "Notes.ReadWrite" };
        private AuthenticationResult authResult = null;
        public static object HttpUtils { get; private set; }
        private string fileName = "";
        private string patientName = "";

        private static readonly string dbPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "ExpressEMR.db");
        private string selectedFileName = "";
        private string dbName1 = "ExpressEMR.db";

        private ObservableCollection<Patient> patientCollection = new ObservableCollection<Patient>();
        private ObservableCollection<Patient> patCollection = new ObservableCollection<Patient>();
        private ObservableCollection<Templates> templatesCollection = new ObservableCollection<Templates>();

        Dictionary<RowColumnIndex, object> committedValues = new Dictionary<RowColumnIndex, object>();
     
        private IEnumerable<Patient> pat;

        private GridSelectedCellsInfo cellInfo = null;

        public MainPage()
        {
            this.InitializeComponent();
            this.CreateOneNotePageButton.Visibility = Visibility.Collapsed;
            this.CreateSectionButton.Visibility = Visibility.Collapsed;

            CopyDatabase();

            DataSetup();





        }


        private void Model_CommitCellInfo(object sender, Syncfusion.UI.Xaml.CellGrid.Styles.GridQueryCellInfoEventArgs e)
        {
            // save the updated cell value into dictionary,

            if (e.Style.HasCellValue)
            {
                committedValues[e.Cell] = e.Style.CellValue;
                e.Handled = true;
            }
        }

        private async Task CopyDatabase()
        {
            bool isDatabaseExisting = false;

            try
            {
                StorageFile storageFile = await ApplicationData.Current.LocalFolder.GetFileAsync("ExpressEMR.db");
                isDatabaseExisting = true;
                Debug.WriteLine("DB exists");
            }
            catch
            {
                isDatabaseExisting = false;
                Debug.WriteLine("DB not  exists");
            }

            if (!isDatabaseExisting)
            {
                StorageFile databaseFile = await Package.Current.InstalledLocation.GetFileAsync("ExpressEMR.db");
                await databaseFile.CopyAsync(ApplicationData.Current.LocalFolder, "ExpressEMR.db", NameCollisionOption.ReplaceExisting);
                Debug.WriteLine("DB copied");
            }
        }

        private void DataSetup()
        {
            //int recCtr = 0;
            var root = Windows.Storage.ApplicationData.Current.LocalFolder.Path;
            using (var db = new SQLite.SQLiteConnection(dbPath))
            {
                try
                {
                    var list = db.Table<Patient>().ToList();
                    foreach (var item in list)
                    {
                        /// Full Patient Table fields displayed
                        patCollection.Add(new Patient() { FirstName = item.FirstName, LastName = item.LastName, Address1 = item.Address1, City = item.City, State = item.State, PostCode = item.PostCode, BirthDate = item.BirthDate });
                    }
                }
                catch { }
            }

            int colCtr = PatientGrid.Columns.Count;

            PatientGrid.ItemsSource = patCollection;

            PatientGrid.Columns[0].IsHidden = true;

            ////PatientGrid.Columns.RemoveAt(1);

            using (var db = new SQLite.SQLiteConnection(dbPath))
            {
                try
                {
                    var list = db.Table<Templates>().ToList();
                    foreach (var item in list)
                    {
                        templatesCollection.Add(new Templates() { TemplateFile = item.TemplateFile, TemplateName = item.TemplateName, Category = item.Category });
                    }
                }
                catch { }
            }

            TemplateGrid.ItemsSource = templatesCollection;

            //   TemplateGrid.Columns[2].IsHidden = true;

            int tempCtr = TemplateGrid.Columns.Count;
        }

        private static SQLiteConnection DbConnection
        {
            get
            {
                return new SQLiteConnection(Path.Combine(ApplicationData.Current.LocalFolder.Path, "Storage.sqlite"));
            }
        }

        //protected override async void OnNavigatedTo(NavigationEventArgs e)
        //{
        //    await SignInHelper();
        //}

        //public async Task<bool> SignInHelper()
        //{
        //    bool flagSignedIn = false;

        //    try
        //    {
        //        authResult = await App.PublicClientApp.AcquireTokenAsync(scopes);

        //        if (authResult != null)
        //        {
        //            DisplayParseAuthResult(authResult);

        //            // For Original MSAL GetRequest
        //            //       string response = await SendGetRequest(authResult.AccessToken);

        //            //For OneNote API Post To Create a Simple Page
        //            // string response = await SendPostRequest(authResult.AccessToken);

        //            // DisplayAPIResponse(response);
        //        }
        //    }
        //    catch (MsalServiceException MSALex)
        //    {
        //        StatusMsg.Text = MSALex.Message;
        //    }
        //    catch (Exception ex)
        //    {
        //        StatusMsg.Text = ex.Message;
        //    }

        //    flagSignedIn = true;
        //    return flagSignedIn;
        //}

        public void DisplayParseAuthResult(AuthenticationResult _AuthResult)
        {
            StatusMsg.Text += string.Format("User Name: {0} \nUserId: {1} \nAccess Token: {2} \n",
                                             _AuthResult.User.Name, _AuthResult.User.DisplayableId, _AuthResult.AccessToken);
        }

        private async void SignInButton_Click(object sender, RoutedEventArgs e)
        {
            StatusMsg.Text = string.Empty;
            // TokenInfoText.Text = string.Empty;

            try
            {
                authResult = await App.PublicClientApp.AcquireTokenSilentAsync(scopes, App.PublicClientApp.Users.FirstOrDefault());
                //  authResult = await App.PublicClientApp.AcquireTokenAsync(scopes,App.PublicClientApp.Users.ToString());
            }
            catch (MsalUiRequiredException ex)
            {
                // A MsalUiRequiredException happened on AcquireTokenSilentAsync. This indicates you need to call AcquireTokenAsync to acquire a token
                System.Diagnostics.Debug.WriteLine($"MsalUiRequiredException: {ex.Message}");

                try
                {
                    authResult = await App.PublicClientApp.AcquireTokenAsync(scopes);
                }
                catch (MsalException msalex)
                {
                    StatusMsg.Text = $"Error Acquiring Token:{System.Environment.NewLine}{msalex}";
                }
            }
            catch (Exception ex)
            {
                StatusMsg.Text = $"Error Acquiring Token Silently:{System.Environment.NewLine}{ex}";
                return;
            }

            if (authResult != null)
            {
                //  ResultText.Text = await GetHttpContentWithToken(graphAPIEndpoint, authResult.AccessToken);
                //   DisplayBasicTokenInfo(authResult);
                this.CreateOneNotePageButton.Visibility = Visibility.Visible;
                this.CreateSectionButton.Visibility = Visibility.Visible;
            }
        }

        private async void CreateSectionButton_Click(object sender, RoutedEventArgs e)
        {
            //From apigee   https://www.onenote.com/api/v1.0/me/notes/sections/0-50FCF51311CEE68D!11537/pages
            //      < head >
            //< title > Test Emr Page from OneNote API console </ title >

            //   < meta name = "created" content = "2014-03-17T09:00:00-08:00" />

            //    </ head >

            //  StatusMsg.Text = await PostHttpContentWithImage(graphAPIEndpoint, authResult.AccessToken);

            string tempPatientName = PatientTextBlock.Text;
            string tempTemplateName = TemplateTextBlock.Text;
            string tempSectionName = "https://www.onenote.com/api/v1.0/me/notes/sections/0-50FCF51311CEE68D!11537/pages";
            string modifiedAPIEndpoint = "https://graph.microsoft.com/v1.0/me/onenote/sections/0-50FCF51311CEE68D!11537/pages";

            //NotebookID for EMR
            string tempNotebookId = "0-50FCF51311CEE68D!11533";

            StatusMsg.Text = await CreateSectionImage(tempSectionName, modifiedAPIEndpoint, tempTemplateName);
        }

        private async Task<string> CreateSectionImage(string noteBookID, string tempSectionName, string fName)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authResult.AccessToken);

            string imagePartName = fName.ToString();
            string date = GetDate();

            string[] tempfName = fName.Split('.');

            string[] tempDate = date.Split('T');

            string simpleHtml = "<html>" +
                                "<head>" +
                                "<title> " + tempfName[0].ToString() + "  " + tempDate[0].ToString() + "</title>" +
                                "<meta name=\"created\" content=\"" + date + "\" />" +
                                "</head>" +
                                "<body>" +
                                "<h1>This is a page with an image on it</h1>" +
                                "<img src=\"name:" + imagePartName + "\" alt=\"A beautiful logo\" width=\"600\" height=\"900\" />" +
                                "</body>" +
                                "</html>";
            // Create the image part - make sure it is disposed after we've sent the message in order to close the stream.
            HttpResponseMessage response;

            //  using (var imageContent = new StreamContent(await GetBinaryStream("assets\\SOAP.jpg")))

            string tempBinaryStream = "assets\\" + imagePartName;
            using (var imageContent = new StreamContent(await GetBinaryStream(tempBinaryStream)))
            {
                imageContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
                HttpRequestMessage createMessage = new HttpRequestMessage(HttpMethod.Post, tempSectionName)
                {
                    Content = new MultipartFormDataContent
                    {
                        {new StringContent(simpleHtml, System.Text.Encoding.UTF8, "text/html"), "Presentation"},{imageContent, imagePartName}
                    }
                };
                // Must send the request within the using block, or the image stream will have been disposed.
                response = await client.SendAsync(createMessage);
            }

            return response.ToString();
        }

        public async Task<string> PostHttpContentWithImage(string url, string token)
        {
            var client = new HttpClient();

            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            // Not adding the Authentication header would produce an unauthorized call and the API will return a 401
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            const string imagePartName = "assets\\SOAP.jpg";
            string date = GetDate();
            string simpleHtml = "<html>" +
                                "<head>" +
                                "<title>A simple page created with an image on it</title>" +
                                "<meta name=\"created\" content=\"" + date + "\" />" +
                                "</head>" +
                                "<body>" +
                                "<h1>This is a page with an ximage on it</h1>" +
                                "<img src=\"name:" + imagePartName + "\" alt=\"A beautiful logo\" width=\"426\" height=\"68\" />" +
                                "</body>" +
                                "</html>";

            // Create the image part - make sure it is disposed after we've sent the message in order to close the stream.
            HttpResponseMessage response;
            using (var imageContent = new StreamContent(await GetBinaryStream("assets\\SOAP.jpg")))
            {
                imageContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
                HttpRequestMessage createMessage = new HttpRequestMessage(HttpMethod.Post, url)
                {
                    Content = new MultipartFormDataContent
                    {
                        {new StringContent(simpleHtml, System.Text.Encoding.UTF8, "text/html"), "Presentation"},{imageContent, imagePartName}
                    }
                };
                // Must send the request within the using block, or the image stream will have been disposed.
                response = await client.SendAsync(createMessage);
            }

            var content = response.ToString();
            return content;
        }

        private async void CreateOneNotePageButton_Click(object sender, RoutedEventArgs e)
        {
            //  StatusMsg.Text = await PostHttpContentWithImage(graphAPIEndpoint, authResult.AccessToken);

            fileName = "SOAP.jpg";

            StatusMsg.Text = await CreatePageImage(fileName);
        }

        private async Task<string> CreatePageImage(string fName)
        {
            var client = new HttpClient();
            // Note: API only supports JSON return type.
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            // This allows you to see what happens when an unauthenticated call is made.
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authResult.AccessToken);

            string imagePartName = fName;

            // const string imagePartName = "SOAP.jpg";
            string date = GetDate();
            string simpleHtml = "<html>" +
                                "<head>" +
                                "<title>A simple page created with an image1 on it</title>" +
                                "<meta name=\"created\" content=\"" + date + "\" />" +
                                "</head>" +
                                "<body>" +
                                "<h1>This is a page with an image on it</h1>" +
                                "<img src=\"name:" + fName + "\" alt=\"A beautiful logo\" width=\"600\" height=\"900\" />" +
                                "</body>" +
                                "</html>";
            // Create the image part - make sure it is disposed after we've sent the message in order to close the stream.
            HttpResponseMessage response;

            string fileName = "assets\\" + fName.ToString();
            //  using (var imageContent = new StreamContent(await GetBinaryStream("assets\\SOAP.jpg")))
            using (var imageContent = new StreamContent(await GetBinaryStream(fileName)))
            {
                imageContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
                HttpRequestMessage createMessage = new HttpRequestMessage(HttpMethod.Post, graphAPIEndpoint)
                {
                    Content = new MultipartFormDataContent
                    {
                        {new StringContent(simpleHtml, System.Text.Encoding.UTF8, "text/html"), "Presentation"},{imageContent, imagePartName}
                    }
                };
                // Must send the request within the using block, or the image stream will have been disposed.
                response = await client.SendAsync(createMessage);
            }

            return response.ToString();
        }

        private static string GetDate()
        {
            return DateTime.Now.ToString("o");
        }

        private async static Task<Stream> GetBinaryStream(string binaryFile)
        {
            var storageFile = await Package.Current.InstalledLocation.GetFileAsync(binaryFile);
            var storageStream = await storageFile.OpenSequentialReadAsync();
            return storageStream.AsStreamForRead();
        }

        //private async void TemplateListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    ListBoxItem lbi = ((sender as ListBox).SelectedItem as ListBoxItem);

        //    fileName = lbi.Content.ToString();

        //    TemplateTextBlock.Text = fileName;

        //    //     StatusMsg.Text = await CreatePageImage(fileName);
        //}

        private void SignOutButton_Click(object sender, RoutedEventArgs e)
        {
            if (App.PublicClientApp.Users.Any())
            {
                try
                {
                    App.PublicClientApp.Remove(App.PublicClientApp.Users.FirstOrDefault());
                    this.StatusMsg.Text = "User has signed-out";
                    this.CreateOneNotePageButton.Visibility = Visibility.Visible;
                    this.SignOutButton.Visibility = Visibility.Collapsed;
                }
                catch (MsalException ex)
                {
                    StatusMsg.Text = $"Error signing-out user: {ex.Message}";
                }
            }
        }

        //private void PatientListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    ListBoxItem lbi = ((sender as ListBox).SelectedItem as ListBoxItem);

        //    patientName = lbi.Content.ToString();

        //    PatientTextBlock.Text = patientName;
        //}

        private async void GetExamplesButton_Click(object sender, RoutedEventArgs e)
        {
            /// - Getting info for ALL sections the user has is represented by the Uri: https://www.onenote.com/api/v1.0/sections
            ///   private string graphAPIEndpoint =                                    "https://graph.microsoft.com/v1.0/me/onenote/pages";
           // string api = "https://graph.microsoft.com/v1.0/me/onenote/sections";

            string api = "https://graph.microsoft.com/v1.0/me/onenote/notebooks";

            //  StatusMsg.Text = await GetHttpContentSections(api.ToString());

            List<ApiBaseResponse> temp = await GetAllSections(api.ToString());

            foreach (ApiBaseResponse arp in temp)
            {
                // StatusMsg.Text += arp.Id.ToString()+System.Environment.NewLine;
                StatusMsg.Text += arp.Links.OneNoteClientUrl.Href.ToString() + System.Environment.NewLine;
            }

            //   StatusMsg.Text = temp.ToString();
        }

        public async Task<List<ApiBaseResponse>> GetAllSections(string apiRoute)
        {
            var client = new HttpClient();

            // Note: API only supports JSON response.
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            // Not adding the Authentication header would produce an unauthorized call and the API will return a 401
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authResult.AccessToken);

            // Prepare an HTTP GET request to the Sections endpoint
            var createMessage = new HttpRequestMessage(HttpMethod.Get, apiRoute);

            HttpResponseMessage response = await client.SendAsync(createMessage);

            var content = response.ToString();

            List<ApiBaseResponse> temp = await TranslateListOfSectionsResponse(response);

            return temp;
        }

        public async Task<string> GetHttpContentSections(string apiRoute)
        {
            var client = new HttpClient();

            // Note: API only supports JSON response.
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            // Not adding the Authentication header would produce an unauthorized call and the API will return a 401
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authResult.AccessToken);

            // Prepare an HTTP GET request to the Sections endpoint
            var createMessage = new HttpRequestMessage(HttpMethod.Get, apiRoute);

            HttpResponseMessage response = await client.SendAsync(createMessage);

            var content = response.ToString();
            return content;
        }

        /// <summary>
        /// Convert the HTTP response message into a simple structure suitable for apps to process
        /// </summary>
        /// <param name="response">The response to convert</param>
        /// <returns>A simple response</returns>
        private static async Task<List<ApiBaseResponse>> TranslateListOfSectionsResponse(HttpResponseMessage response)
        {
            var apiBaseResponse = new List<ApiBaseResponse>();
            string body = await response.Content.ReadAsStringAsync();
            if (response.StatusCode == HttpStatusCode.OK
                /* GET Sections calls always return 200-OK upon success */)
            {
                var content = JObject.Parse(body);
                apiBaseResponse = new List<ApiBaseResponse>(JsonConvert.DeserializeObject<List<GenericEntityResponse>>(content["value"].ToString()));
            }
            if (apiBaseResponse.Count == 0)
            {
                apiBaseResponse.Add(new ApiBaseResponse());
            }

            // Extract the correlation id.  Apps should log this if they want to collect the data to diagnose failures with Microsoft support
            IEnumerable<string> correlationValues;
            if (response.Headers.TryGetValues("X-CorrelationId", out correlationValues))
            {
                apiBaseResponse[0].CorrelationId = correlationValues.FirstOrDefault();
            }
            apiBaseResponse[0].StatusCode = response.StatusCode;
            apiBaseResponse[0].Body = body;
            return apiBaseResponse;
        }

        private void TemplateGrid_SelectionChanged(object sender, Syncfusion.UI.Xaml.Grid.GridSelectionChangedEventArgs e)
        {
            List<GridCellInfo> selectedCells = this.TemplateGrid.GetSelectedCells();
            //     List<string> selectedCells = e.AddedItems.ToString();

            //   string[] tempStr = e.AddedItems.ToList<Row>

            //    List<GridCellInfo> selectedCells = this.TemplateGrid.GetSelectedCells();
        }

        private void TemplateGrid_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
        }

        private void TemplateGrid_CellTapped(object sender, GridCellTappedEventArgs e)
        {
            //GridControl Grid = sender as GridControl;
            //GridStyleInfoStore data = Grid.Data[e.RowIndex, e.ColIndex] as GridStyleInfoStore;
            //if (data != null)
            //{
            //    GridStyleInfo style = new GridStyleInfo(data);
            //    MyType mytype = style.CellValue as MyType;
            //    if (mytype != null)
            //    {
            //        e.Style.Text = mytype.Name;
            //    }
            //}
            //Console.WriteLine("xxx" + e.Style.Text);

        }

        private void TemplateCellGrid_CellClick(object sender, Syncfusion.UI.Xaml.CellGrid.Helpers.GridCellClickEventArgs e)
        {
            //var cell = TemplateCellGrid.SelectionController.CurrentCell;

        }

        private void TemplateGrid_CurrentCellActivated(object sender, CurrentCellActivatedEventArgs e)
        {
            int rowIndex = e.CurrentRowColumnIndex.RowIndex;
            int columnIndex = this.TemplateGrid.ResolveToGridVisibleColumnIndex(e.CurrentRowColumnIndex.ColumnIndex);
            if (columnIndex < 0)
                return;
            // Get the mapping name
            var mappingName = this.TemplateGrid.Columns[columnIndex].MappingName;
            // Get the resolved current record index
            var recordIndex = this.TemplateGrid.ResolveToRecordIndex(rowIndex);
            if (recordIndex < 0)
                return;
            if (e.ActivationTrigger == ActivationTrigger.Mouse)
            {
                if (this.TemplateGrid.View.TopLevelGroup != null)
                {
                    // Get the current row record while grouping
                    var record = this.TemplateGrid.View.TopLevelGroup.DisplayElements[recordIndex];
                    if (!record.IsRecords) //skips caption summary, group summary rows
                        return;
                    var data = (record as RecordEntry).Data;
                    txtCellValue.Text = data.GetType().GetRuntimeProperty(mappingName).GetValue(data).ToString();
                }
                else
                {
                    // Get the current row record 
                    var record1 = this.TemplateGrid.View.Records.GetItemAt(recordIndex);
                    txtCellValue.Text = record1.GetType().GetRuntimeProperty(mappingName).GetValue(record1).ToString();
                }
            }
        }
    }
}