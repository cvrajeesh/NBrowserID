Overview
=========

Project Home: https://github.com/cvrajeesh/NBrowserID
A Demo Application can be found in https://github.com/cvrajeesh/NBrowserID


How to Use BrowserID in ASP.NET MVC Application
===============================================

1.	Install NBrowserID if you haven’t done it. In the Package Manager console type 
	Install-Package NBrowserID
	
2.	Include the BrowserID javascript into your master page or in the view 
	<script src="@Url.Content("https://browserid.org/include.js")" type="text/javascript"></script>
	
3.	Add a login button link to your view
4.  Hook into its click event and invoke "navigator.id.getVerifiedEmail" method with a call back function
	E.g. 
		 $('#login').click(function () {
            navigator.id.getVerifiedEmail(gotVerifiedEmail);
        });
		
5. In the call back function, pass the assertion back to the MVC application
	E.g. the below code will post the assertion to LogOn method on the AccountController
	
	function gotVerifiedEmail(assertion) {
        // got an assertion, now send it up to the server for verification
        if (assertion !== null) {
            $.ajax({
                type: 'POST',
                url: '/Account/LogOn',
                data: { assertion: assertion },
                success: function (res, status, xhr) {
                    if (res != null) {
                        // Login is successful and verification is completed. Update the UI
                    }
                },
                error: function (res, status, xhr) {
                    alert("login failure" + res);
                }
            });
        }
    }
	
6.  In the server side, verify the assertion that is received using the NBrowserID library. If the verification is successful, mark the user as logged in
	E.g.
		[HttpPost]
        public ActionResult LogOn(string assertion)
        {
            var authentication = new BrowserIDAuthentication();
            var verificationResult = authentication.Verify(assertion);
            if (verificationResult.IsVerified)
            {				
                string email = verificationResult.Email;
                FormsAuthentication.SetAuthCookie(email, false);
                return Json(new { email });
            }

            return Json(null);
        }


