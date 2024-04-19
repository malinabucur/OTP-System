# OTP SYSTEM

## Business requirements fulfilled: 
* The OTP system is secure, the OTPs are generated randomly and are not predictable and encryption is in place.
* The OTPs are time-bound, they are set to expire after 30 seconds.
* The interface is intuitive and user-friendly.
* The system has good error handling and informs the user about any issues.
* The OTP code is displayed in a toast message that is visible as long as the OTP is valid.

![image](https://github.com/malinabucur/OTP-System/assets/109577091/97ea7964-68aa-4968-a8b4-10e6e8130965)
* When clicking on generate OTP code, the code will be displayed in the right bottom corner in a toast message:
![image](https://github.com/malinabucur/OTP-System/assets/109577091/3b15808f-9d6b-46e8-be87-2f92fc9ace24)
* If a valid OTP code is entered in the input field:
![image](https://github.com/malinabucur/OTP-System/assets/109577091/54239657-5775-4227-aa33-169d13109b6b)
* If the OTP codes matches, the user will be redirected to:
![image](https://github.com/malinabucur/OTP-System/assets/109577091/9cfceb35-bae8-4904-81e9-c85cab18d61f)
* If the entered OTP code is invalid, an error toast message will be displayed and visible for 5 seconds:
![image](https://github.com/malinabucur/OTP-System/assets/109577091/56f86fb0-9fbb-4a68-a26c-22a7c5e4e1f0)
* The input field expects only digits, if not, the Submit button is disabled:
![image](https://github.com/malinabucur/OTP-System/assets/109577091/6414eb9e-74f3-4ca6-a31f-d8ab093833e6)



## Technical requirements fulfilles:
* The solution is a web application developed with the latest .NET framework.
* For the frontend part I used JavaScript, HTML and CSS, along with Bootstrap for the styling part. 
* Unit tests were performed. 
