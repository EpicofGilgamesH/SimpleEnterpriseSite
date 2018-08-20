var btnPsm = {
    addBtn: 'AddBtnPms',
    editBtn: "EditBtnPms",
    deleteBtn: "DeleteBtnPms",
    uploadBtn: "UploadBtnPms",
    downloadBtn: "DownLoadBtnPms",
}

function getState(state) {
    if (state == "Delete") {
        return "已禁用";
    } else if (state == "Normal") {
        return "正常";
    } else {
        return "已锁住";
    }
}

function trim(x) {
    return x.toString().replace(/^\s+|\s+$/gm, '');
}

/* 异步请求结果返回状态码 */
var ResultStatus =
    {
        OK: 100,
        Failed: 101,
        NotLogin: 102,
        Unauthorized: 103
    };

/* $ace */
(function ($) {
    var $ace = {};

    /* 返回 json 数据 */
    $ace.get = function (url, data, callback) {
        if ($.isFunction(data)) {
            callback = data;
            data = undefined;
        }

        url = parseUrl(url);
        var ret = execAjax("GET", url, data, callback);
        return ret;
    }
    $ace.post = function (url, data, callback) {
        if ($.isFunction(data)) {
            callback = data;
            data = undefined;
        }

        var ret = execAjax("POST", url, data, callback);
        return ret;
    }

    $ace.alert = function (msg, callBack) {
        layerAlert(msg, callBack);
    }
    $ace.confirm = function (msg, callBack) {
        layerConfirm(msg, callBack);
    }
    $ace.msg = function (msg) {
        layerMsg(msg);
    }

    $ace.reload = function () {
        location.reload();
        return false;
    }

    /* 将当前 url 的参数值 */
    $ace.getQueryParam = function (name) {
        if (name === null || name === undefined || name === "")
            return "";
        name = ("" + name).toLowerCase();
        var search = location.search.slice(1);
        var arr = search.split("&");
        for (var i = 0; i < arr.length; i++) {
            var ar = arr[i].split("=");
            if (ar[0].toLowerCase() == name) {
                if (unescape(ar[1]) == 'undefined') {
                    return "";
                } else {
                    return unescape(ar[1]);
                }
            }
        }
        return "";
    }
    /* 将当前 url 参数转成一个 js 对象 */
    $ace.getQueryParams = function () {
        var params = {};
        var loc = window.location;
        var se = decodeURIComponent(loc.search);

        if (!se) {
            return params;
        }

        var paramsString;
        paramsString = se.substr(1);//将?去掉
        var arr = paramsString.split("&");
        for (var i = 0; i < arr.length; i++) {
            var item = arr[i];
            var index = item.indexOf("=");
            if (index == -1)
                continue;
            var paramName = item.substr(0, index);
            if (!paramName)
                continue;
            var value = item.substr(index + 1);
            params[paramName] = value;
        }
        return params;
    }

    /* optionList: [{"Value" : "1", "Text" : "开发部"},{"Value" : "2", "Text" : "测试部"}] */
    $ace.getOptionTextByValue = function (optionList, value, valuePropName, textPropName) {
        valuePropName = valuePropName || "Value";
        textPropName = textPropName || "Text";

        var text = "";
        var len = optionList.length;
        for (var i = 0; i < len; i++) {
            if (optionList[i][valuePropName] == value) {
                text = optionList[i][textPropName];
                break;
            }
        }

        return text;
    }

    /* 依赖 bootstrap ui */
    $ace.selectRow = function (selectedTr) {
        var c = "warning";
        $(selectedTr).addClass(c);
        $(selectedTr).siblings().removeClass(c);
        return true;
    }
    $ace.formatBool = function (val) {
        if (val == true) {
            return "是";
        }
        else if (val == false) {
            return "否";
        }

        return val;
    }


    function execAjax(type, url, data, callback) {
        var layerIndex = layer.load(1);
        var ret = $.ajax({
            url: url,
            type: type,
            dataType: "json",
            data: data,
            complete: function (xhr) {
                layer.close(layerIndex);
            },
            success: function (result) {
                var isStandardResult = ("Status" in result) && ("Msg" in result);
                if (isStandardResult) {
                    if (result.Status == ResultStatus.Failed) {
                        layerAlert(result.Msg || "操作失败");
                        return;
                    }
                    if (result.Status == ResultStatus.NotLogin) {
                        layerAlert(result.Msg || "未登录或登录过期，请重新登录");
                        return;
                    }
                    if (result.Status == ResultStatus.Unauthorized) {
                        layerAlert(result.Msg || "权限不足，禁止访问");
                        return;
                    }

                    if (result.Status == ResultStatus.OK) {
                        /* 传 result，用 result.Data 还是 result.Msg，由调用者决定 */
                        callback(result);
                    }
                }
                else
                    callback(result);
            },
            error: errorCallback
        });
        return ret;
    }
    function errorCallback(xhr, textStatus, errorThrown) {
        var json = { textStatus: textStatus, errorThrown: errorThrown };
        alert("请求失败: " + JSON.stringify(json));
    }
    function parseUrl(url) {
        if (url.indexOf("_dc=") < 0) {
            if (url.indexOf("?") >= 0) {
                url = url + "&_dc=" + (new Date().getTime());
            } else {
                url = url + "?_dc=" + (new Date().getTime());
            }
        }
        return url;
    };

    function layerAlert(msg, callBack) {
        msg = msg == null ? "" : msg;/* layer.alert 传 null 会报错 */
        var type = 'warning';
        var icon = "";
        if (type == 'success') {
            icon = "fa-check-circle";
        }
        if (type == 'error') {
            icon = "fa-times-circle";
        }
        if (type == 'warning') {
            icon = "fa-exclamation-circle";
        }

        var idx;
        idx = layer.alert(msg, {
            icon: icon,
            title: "系统提示",
            btn: ['确认'],
            btnclass: ['btn btn-primary'],
        }, function () {
            layer.close(idx);
            if (callBack)
                callBack();
        });
    }
    function layerConfirm(content, callBack) {
        var idx;
        idx = layer.confirm(content, {
            icon: "fa-exclamation-circle",
            title: "系统提示",
            btn: ['确认', '取消'],
            btnclass: ['btn btn-primary', 'btn btn-danger'],
        }, function () {
            layer.close(idx);
            callBack();
        }, function () {

        });
    }
    function layerMsg(msg, callBack) {
        msg = msg == null ? "" : msg;/* layer.msg 传 null 会报错 */
        layer.msg(msg, { time: 2000, shift: 0 });
    }


    window.$ace = $ace;
})($);
