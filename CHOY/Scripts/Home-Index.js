////////////生日日期限制/////////////////////////
var today = new Date();
var dd = today.getDate();
var mm = today.getMonth() + 1; //January is 0!
var yyyy = today.getFullYear();
if (dd < 10) {
	dd = '0' + dd;
}
if (mm < 10) {
	mm = '0' + mm;
}

today = yyyy + '-' + mm + '-' + dd;
document.getElementById("Birthday").setAttribute("max", today);




// 補充 by Jian Fong
function getUrlParameter(name) {
	name = name.replace(/[\[]/, '\\[').replace(/[\]]/, '\\]');
	var regex = new RegExp('[\\?&]' + name + '=([^&#]*)');
	var results = regex.exec(location.search);
	return results === null ? '' : decodeURIComponent(results[1].replace(/\+/g, ' '));
};

function checkVerifyEmailAddressToken() {
	let token = getUrlParameter("token");
	if (token == "") {
		document.location.href = "/";
	}
}
// 失去焦点
function handleBlurOrInputForSetPwd() {
	const pwd = document.getElementById("PasswordForSet").value;
	const confirmPwd = document.getElementById("confirmPwd").value;
	const errorMsg = document.getElementById("errorMessagesForSetPassword");
	if (pwd.length < 6 || pwd.length > 18) {
		errorMsg.innerText = "密碼長度必須界在 6 ~ 18 個字元";
	} else if (pwd !== confirmPwd) {
		errorMsg.innerText = "兩次輸入的密碼必須一致";
	} else {
		errorMsg.innerText = "";
	}
}
document.getElementById("PasswordForSet").addEventListener("blur", () => handleBlurOrInputForSetPwd());
document.getElementById("confirmPwd").addEventListener("blur", () => handleBlurOrInputForSetPwd());
document.getElementById("PasswordForSet").addEventListener("input", () => handleBlurOrInputForSetPwd());
document.getElementById("confirmPwd").addEventListener("input", () => handleBlurOrInputForSetPwd());
// 焦点
function handleFocusForSetPwd() {
	const errorMsg = document.getElementById("errorMessagesForSetPassword");
	errorMsg.innerText = "";
}
document.getElementById("PasswordForSet").addEventListener("focus", () => handleFocusForSetPwd());
document.getElementById("confirmPwd").addEventListener("focus", () => handleFocusForSetPwd());



function handleRegister() {
	const pwd = document.getElementById("PasswordForSet").value;
	const confirmPwd = document.getElementById("confirmPwd").value;
	const errorMsg = document.getElementById("errorMessagesForSetPassword");
	if (pwd.length < 6 || pwd.length > 18) {
		errorMsg.innerText = "密碼長度必須界在 6 ~ 18 個字元";
		return false;
	}
	if (pwd !== confirmPwd) {
		errorMsg.innerText = "兩次輸入的密碼必須一致";
		return false;
	}
	const settings = {
		"url": "/api/auth/register",
		"method": "POST",
		"timeout": 0,  
		"headers": { "Content-Type": "application/json"	},
		"dataType": "json",
		"data": JSON.stringify({ 
			"Token": getUrlParameter("token"),
			"Password": pwd 
		}),
	};

	$.ajax(settings).done(function (response) {
		console.log(response);
		if (response.Success) {
			alert(response.Messages);
			document.location.href = "/";
		}
	}).fail(function(response){
		console.log(response);
		switch (response.status) {
			case 400:
			case 401:
				errorMsg.innerText = response.responseJSON.Messages;
				break;
			default:
				errorMsg.innerText = "網站忙碌中，請稍後再試 !!!";
				break;
		}
	});
	return false;
}
// 寄送
// 失去焦点
function handleBlurOrInputForRegister() {
	const email = document.getElementById("Email").value;
	const birthday = document.getElementById("Birthday").value;
	const errorMsg = document.getElementById("errorMessagesForRegister");
	emailRule = /^\w+((-\w+)|(\.\w+))*\@[A-Za-z0-9]+((\.|-)[A-Za-z0-9]+)*\.[A-Za-z]+$/;
	if (!emailRule.test(email)) {
		errorMsg.innerText = "Email格式錯誤!!";
	} else if (birthday === "")	{
		errorMsg.innerText = "Birthday不能為空";
	} else {
		errorMsg.innerText = "";
	}
}
document.getElementById("Email").addEventListener("blur", () => handleBlurOrInputForRegister())
document.getElementById("UserName").addEventListener("blur", () => handleBlurOrInputForRegister())
document.getElementById("Birthday").addEventListener("blur", () => handleBlurOrInputForRegister())
document.getElementById("Email").addEventListener("input", () => handleBlurOrInputForRegister())
document.getElementById("Birthday").addEventListener("input", () => handleBlurOrInputForRegister())
// 焦点
function handleFocusForRegister() {
	const errorMsg = document.getElementById("errorMessagesForRegister");
	errorMsg.innerText = "";
}
document.getElementById("Email").addEventListener("focus", () => handleFocusForRegister());
document.getElementById("UserName").addEventListener("focus", () => handleFocusForRegister());
document.getElementById("Birthday").addEventListener("focus", () => handleFocusForRegister());

function handleVerifyEmailAddress() {
	const submitBtn = document.querySelector("#page_reg input[type=submit]")
	submitBtn.disabled = true;
	const email = document.getElementById("Email").value;
	const userName = document.getElementById("UserName").value;
	const gender = [...document.querySelectorAll("input[type=radio][name=Gender]")].filter(item => item.checked)[0].value === "1";
	const birthday = document.getElementById("Birthday").value;
	const agree = document.getElementById("agree").checked;
	const errorMsg = document.getElementById("errorMessagesForRegister");


	if (email === "") {
		errorMsg.innerText = "Email不能為空";
		submitBtn.disabled = false;
		return false;
	}

	if (userName !== "" && userName.length > 15)	{
		errorMsg.innerText = "Username不能超過15個字";
		submitBtn.disabled = false;
		return false;
	}
	if (birthday === "")	{
		errorMsg.innerText = "Birthday不能為空";
		submitBtn.disabled = false;
		return false;
	}
	if (!agree)	{
		errorMsg.innerText = "請點選我同意條款";
		submitBtn.disabled = false;
		return false;
	}
	errorMsg.innerText = "";

	const data = {
		Email: email,
		Gender: gender,
		Birthday: birthday
	}
	if (userName !== "") {
		data["UserName"] = userName;
	}
	const settings = {
		"url": "/api/auth/verifyEmailAddress",
		"method": "POST",
		"timeout": 0,
		"headers": {
			"Content-Type": "application/json",
		},
		"dataType": "json",
		"data": JSON.stringify(data),
	};

	$.ajax(settings).done(function (response) {
		console.log(response);
		if (response.Success) {
			alert(response.Messages);
			document.location.href = "/";
			submitBtn.disabled = false;
		}
	}).fail(function(response) {
		console.log(response);
		switch (response.status) {
			case 400:
				errorMsg.innerText = response.responseJSON.Messages;
				break;
			default:
				errorMsg.innerText = "網站忙碌中，請稍後再試 !!!";
				break;
		}
		submitBtn.disabled = false;
	});
	return false;
}
// Login page
// 失去焦點
function handleBlurOrInputForLogin() {
	const errorMsg = document.getElementById("errorMessagesForLogin");
	const password = document.getElementById("PasswordForLogin").value;
	if (password.length < 6 || password.length > 18) {
		errorMsg.innerText = "密碼格式錯誤!!!";
	} else {
		errorMsg.innerText = "";
	}
}
document.getElementById("Account").addEventListener("blur", () => handleBlurOrInputForLogin());
document.getElementById("PasswordForLogin").addEventListener("blur", () => handleBlurOrInputForLogin());
document.getElementById("PasswordForLogin").addEventListener("input", () => handleBlurOrInputForLogin());
// 焦点
function handleFocusForLogin() {
	const errorMsg = document.getElementById("errorMessagesForLogin");
	errorMsg.innerText = "";
}
document.getElementById("Account").addEventListener("focus", () => handleFocusForLogin());
document.getElementById("PasswordForLogin").addEventListener("focus", () => handleFocusForLogin());
function handleLogin() {
	const account = document.getElementById("Account").value;
	const password = document.getElementById("PasswordForLogin").value;
	const errorMsg = document.getElementById("errorMessagesForLogin");
	if (account.length === 0 || password.length === 0)
	{
		errorMsg.innerText = "請輸入帳號或密碼!!!"
		return false;
	}
	if (password.length < 6 || password.length > 18) {
		errorMsg.innerText = "密碼格式錯誤!!!";
		return false;
	}

	const settings = {
		"url": "/api/auth/login",
		"method": "POST",
		"timeout": 0,
		"headers": {
			"Content-Type": "application/json",
		},
		// "dataType": "json",
		"data": JSON.stringify({
			Account: account,
			Password: password
		}),
	};

	$.ajax(settings).done(function (response) {
		if (response.Success) {
			alert(response.Messages);
			document.location.href = "/";
		} 
	}).fail(function(response) {
		switch (response.status) {
			case 404:
				errorMsg.innerText = response.responseJSON.Messages;
				break;
			default:
				errorMsg.innerText = "網站忙碌中，請稍後再試 !!!";
				break;
		}
	});
	return false;
}
