////////////////////nav裡的按鈕切換////////////////////////////////////////////
var element = document.getElementsByClassName("nav-btn-box")[0];
element.addEventListener("click", myFunction);
function myFunction(e) {
	var elems = document.querySelector(".non-active");
	if (elems != null) {
		elems.classList.remove("non-active");
	}
	e.target.classList.add("non-active");
}

/////////////////頁面跳轉(利用 #)////////////////////////
var page = document.getElementsByClassName('page-container')[0];
var page_reg = document.getElementById("page_reg");
var page_home = document.getElementById("page_home");
var page_login = document.getElementById("page_login");
var page_check = document.getElementById("page_check");

function renderByUrl(url) {
	var active = document.querySelector(".is-active");
	if (active != null) {
		active.classList.remove("is-active");
	}
	//若沒有hash值預設導向至#/
	if (location.hash === "") {
		location.href = "#/home";
		return;
	}
	//路徑對應頁面內容
	if (url === "#/" || url === "#/home") {
		page_home.classList.add('is-active');
	} else if (url === "#/login") {
		page_login.classList.add('is-active');
	} else if (url === "#/register") {
		page_reg.classList.add('is-active');
	} else if (url === "#/check") {
		page_check.classList.add('is-active');
		checkVerifyEmailAddressToken();
	}
}

//載入事件
window.addEventListener("load", function () {
	//使用location.hash判斷頁面內容
	renderByUrl(location.hash);
	console.log(location.hash);
});

//監聽網址改變事件
window.addEventListener("hashchange", function () {
	//使用location.hash判斷頁面內容
	renderByUrl(location.hash);
	console.log(location.hash);
});



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
		errorMsg.innerText = "Your password must be between 6 and 18 characters";
	} else if (pwd !== confirmPwd) {
		errorMsg.innerText = "You must enter the same password twice in order to confirm it";
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
		errorMsg.innerText = "Your password must be between 6 and 18 characters";
		return false;
	}
	if (pwd !== confirmPwd) {
		errorMsg.innerText = "You must enter the same password twice in order to confirm it";
		return false;
	}
	const settings = {
		"url": "/api/auth/register",
		"method": "POST",
		"timeout": 0,
		"headers": { "Content-Type": "application/json" },
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
	}).fail(function (response) {
		console.log(response);
		switch (response.status) {
			case 400:
			case 401:
				errorMsg.innerText = response.responseJSON.Messages;
				break;
			default:
				errorMsg.innerText = "Sorry, the server is busy. Please try again later";
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
		errorMsg.innerText = "Wrong email format.";
	} else if (birthday === "") {
		errorMsg.innerText = "Birthday is required";
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
		errorMsg.innerText = "Email is required.";
		submitBtn.disabled = false;
		return false;
	}

	if (userName !== "" && userName.length > 15) {
		errorMsg.innerText = "Your nickname cannot be longer than 15 characters.";
		submitBtn.disabled = false;
		return false;
	}
	if (birthday === "") {
		errorMsg.innerText = "Birthday is required.";
		submitBtn.disabled = false;
		return false;
	}
	if (!agree) {
		errorMsg.innerText = 'Please tick the box "I have read and accept  the terms and conditions" to indicate your consent.';
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
	document.querySelector('.ani').classList.toggle('hidden')
	$.ajax(settings).done(function (response) {
		document.querySelector('.ani').classList.toggle('hidden')
		console.log(response);
		if (response.Success) {
			alert(response.Messages);
			document.location.href = "/";
			submitBtn.disabled = false;
		}
	}).fail(function (response) {
		document.querySelector('.ani').classList.toggle('hidden')
		console.log(response);
		switch (response.status) {
			case 400:
				errorMsg.innerText = response.responseJSON.Messages;
				break;
			default:
				errorMsg.innerText = "Sorry, the server is busy. Please try again later.";
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
		errorMsg.innerText = "Wrong password format";
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
	if (account.length === 0 || password.length === 0) {
		errorMsg.innerText = "Enter your email and password"
		return false;
	}
	if (password.length < 6 || password.length > 18) {
		errorMsg.innerText = "Wrong password format";
		return false;
	}

	const settings = {
		"url": "/api/auth/login",
		"method": "POST",
		"timeout": 0,
		"headers": { "Content-Type": "application/json" },
		"dataType": "json",
		"data": JSON.stringify({
			Account: account,
			Password: password
		}),
	};

	$.ajax(settings).done(function (response) {
		if (response.Success) {
			alert(response.Messages);

			document.location.href = "/Project/Index";
		}
	}).fail(function (response) {
		switch (response.status) {
			case 404:
				errorMsg.innerText = response.responseJSON.Messages;
				break;
			default:
				errorMsg.innerText = "Sorry, the server is busy. Please try again later.";
				break;
		}
	});
	return false;
}
