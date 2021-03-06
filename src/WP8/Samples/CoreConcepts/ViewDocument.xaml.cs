// ------------------------------------------------------------------------------
// Copyright (c) 2014 Microsoft Corporation
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
//  of this software and associated documentation files (the "Software"), to deal
//  in the Software without restriction, including without limitation the rights
//  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//  copies of the Software, and to permit persons to whom the Software is
//  furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
//  all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//  THE SOFTWARE.
// ------------------------------------------------------------------------------

using System;
using System.Windows;
using System.Windows.Navigation;

using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;
using Microsoft.Live;

namespace CoreConcepts
{
    public partial class ViewDocument : PhoneApplicationPage
    {
        private LiveConnectClient liveClient;
        private string name;
        private string id;

        public ViewDocument()
        {
            InitializeComponent();
            this.Loaded += this.OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            this.Loaded -= this.OnLoaded;

            if (MainPage.Session == null)
            {
                this.NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
            }
            else
            {
                this.liveClient = new LiveConnectClient(MainPage.Session);
                this.ButtonView.IsEnabled = true;
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            this.name = this.NavigationContext.QueryString["name"];
            this.id = this.NavigationContext.QueryString["id"];

            this.TextBlockName.Text = this.name;
        }

        private async void ButtonView_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                LiveOperationResult operationResult = await this.liveClient.GetAsync(this.id + "/shared_read_link");
                dynamic properties = operationResult.Result;
                if (properties.link != null)
                {
                    var browser = new WebBrowserTask();

                    browser.Uri = new Uri(properties.link, UriKind.Absolute);
                    browser.Show();
                }
                else
                {
                    this.ShowError("Could not find the 'link' attribute.");
                }
            }
            catch (LiveConnectException exception)
            {
                this.ShowError(exception.Message);
            }
        }

        private void ShowError(string message)
        {
            this.NavigationService.Navigate(new Uri("/Error.xaml?msg=" + message, UriKind.Relative));
        }
    }
}