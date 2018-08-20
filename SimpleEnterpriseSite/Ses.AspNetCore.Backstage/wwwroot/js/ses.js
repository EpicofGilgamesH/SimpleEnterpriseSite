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

//转换日期格式(时间戳转换为datetime格式)
function changeDateFormat(cellval) {
    var dateVal = cellval + "";
    if (cellval != null) {
        var date = new Date(parseInt(dateVal.replace("/Date(", "").replace(")/", ""), 10));
        var month = date.getMonth() + 1 < 10 ? "0" + (date.getMonth() + 1) : date.getMonth() + 1;
        var currentDate = date.getDate() < 10 ? "0" + date.getDate() : date.getDate();

        var hours = date.getHours() < 10 ? "0" + date.getHours() : date.getHours();
        var minutes = date.getMinutes() < 10 ? "0" + date.getMinutes() : date.getMinutes();
        var seconds = date.getSeconds() < 10 ? "0" + date.getSeconds() : date.getSeconds();

        return date.getFullYear() + "-" + month + "-" + currentDate + " " + hours + ":" + minutes + ":" + seconds;
    }
}

/* 异步请求结果返回状态码 */
var ResultStatus =
    {
        OK: 100,
        Failed: 101,
        NotLogin: 102,
        Unauthorized: 103
    };

/* $ses */
(function ($) {
    var $ses = {};
    /*将时间戳转换成日期格式 */
    $ses.formatDate = function (cellval) {
        debugger;
        var dateVal = cellval + "";
        if (cellval != null) {
            var date = new Date(parseInt(dateVal.replace("/Date(", "").replace(")/", ""), 10));
            var month = date.getMonth() + 1 < 10 ? "0" + (date.getMonth() + 1) : date.getMonth() + 1;
            var currentDate = date.getDate() < 10 ? "0" + date.getDate() : date.getDate();

            var hours = date.getHours() < 10 ? "0" + date.getHours() : date.getHours();
            var minutes = date.getMinutes() < 10 ? "0" + date.getMinutes() : date.getMinutes();
            var seconds = date.getSeconds() < 10 ? "0" + date.getSeconds() : date.getSeconds();

            return date.getFullYear() + "-" + month + "-" + currentDate + " " + hours + ":" + minutes + ":" + seconds;
        }
    }

    /* 返回 json 数据 */
    $ses.get = function (url, data, callback) {
        if ($.isFunction(data)) {
            callback = data;
            data = undefined;
        }

        url = parseUrl(url);
        var ret = execAjax("GET", url, data, callback);
        return ret;
    }
    $ses.post = function (url, data, callback) {
        if ($.isFunction(data)) {
            callback = data;
            data = undefined;
        }

        var ret = execAjax("POST", url, data, callback);
        return ret;
    }

    $ses.alert = function (msg, callBack) {
        layerAlert(msg, callBack);
    }
    $ses.confirm = function (msg, callBack) {
        layerConfirm(msg, callBack);
    }
    $ses.msg = function (msg) {
        layerMsg(msg);
    }

    $ses.reload = function () {
        location.reload();
        return false;
    }

    /* 将当前 url 的参数值 */
    $ses.getQueryParam = function (name) {
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
    $ses.getQueryParams = function () {
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
    $ses.getOptionTextByValue = function (optionList, value, valuePropName, textPropName) {
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
    $ses.selectRow = function (selectedTr) {
        var c = "warning";
        $(selectedTr).addClass(c);
        $(selectedTr).siblings().removeClass(c);
        return true;
    }
    $ses.formatBool = function (val) {
        if (val == true) {
            return "是";
        }
        else if (val == false) {
            return "否";
        }

        return val;
    }

    $ses.layerWindow = function (title, content) {
        var index = layer.open({
            type: 1,
            title: title,
            skin: 'layui-layer-rim', //样式类名
            area: ['1000px', '800px'], //宽高
            closeBtn: 0, //不显示关闭按钮
            anim: 2,
            shadeClose: true, //开启遮罩关闭
            content: $(content),
        });
        return index;
    };

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

    //下载文档 需要使用form 而非ajax
    /*===================post请求下载文件
     * options:{
     * url:'',  //下载地址
     * data:{name:value}, //要发送的数据
     * method:'post'
     * }
     */

    $ses.postExportFile = function (options) {
        var config = $.extend(true, { method: 'post' }, options);
        var $form = $("<form>");
        $form.attr('style', 'display:none');
        $form.attr('target', '');
        $form.attr('method', config.method);
        $form.attr('action', config.url);
        for (var key in config.data) {
            $form.append('<input type="hidden" name="' + key + '" value"' + config.data[key] + '"/>');
        }
        $('body').append($form);
        $form.submit();
        $form.remove();
    }



    window.$ses = $ses;
})($);




