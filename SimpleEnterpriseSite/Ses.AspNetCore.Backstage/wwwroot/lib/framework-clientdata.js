var clients = [];
$(function () {
    clients = $.clientsInit();
})
$.clientsInit = function () {
    var dataJson = {
        dataItems: [],
        organize: [],
        role: [],
        duty: [],
        user: [],
        authorizeMenu: [],
        authorizeButton: []
    };
    var init = function () {
        $.ajax({
            url: "/Home/GetMenusJsonData",
            type: "get",
            dataType: "json",
            async: false,
            success: function (data) {
                dataJson.dataItems = data.dataItems;
                dataJson.organize = data.organize;
                dataJson.role = data.role;
                dataJson.duty = data.duty;
                dataJson.authorizeMenu = eval(data.Data.authorizeMenu);
                dataJson.authorizeButton = data.authorizeButton;
            }
        });
    }
    init();
    return dataJson;
}