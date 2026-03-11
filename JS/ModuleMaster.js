function Logout() {
    sessionStorage.clear();
    window.location.href = '../Login.aspx'
}

//$('#LogoutBtn').click(() => {
//    sessionStorage.clear();
//    window.location.href = '../Login.aspx'
//})