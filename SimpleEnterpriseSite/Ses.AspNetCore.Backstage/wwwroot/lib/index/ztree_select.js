var setting = {
    view: {
        dblClickExpand: false,
        selectedMulti: false
    },
    data: {
        simpleData: {
            enable: true
        }
    },
    callback: {
        //beforeClick: beforeClick,
        onClick: onClick
    }
};

var zNodes = [];

function beforeClick(treeId, treeNode) {
    var check = (treeNode && !treeNode.isParent);
    if (!check) alert("只能选择城市...");
    return check;
}

function onClick(e, treeId, treeNode) {
    var zTree = $.fn.zTree.getZTreeObj("treeDemo"),
        nodes = zTree.getSelectedNodes(),
        v = "";
    nodes.sort(function compare(a, b) { return a.id - b.id; });
    for (var i = 0, l = nodes.length; i < l; i++) {
        v += nodes[i].name + ",";
    }
    if (v.length > 0) v = v.substring(0, v.length - 1);
    var cityObj = $("#citySel");
    cityObj.attr("value", v);
    //保存id值 只能单选
    cityObj.attr("data-id", nodes[0].id);
}

function showMenu() {
    var cityObj = $("#citySel");
    var cityOffset = $("#citySel").offset();
    $("#menuContent").css({ left: cityOffset.left + "px", top: cityOffset.top + cityObj.outerHeight() + "px" }).slideDown("fast");

    $("body").bind("mousedown", onBodyDown);
}
function hideMenu() {
    $("#menuContent").fadeOut("fast");
    $("body").unbind("mousedown", onBodyDown);
}
function onBodyDown(event) {
    if (!(event.target.id == "menuBtn" || event.target.id == "menuContent" || $(event.target).parents("#menuContent").length > 0)) {
        hideMenu();
    }
}

$(document).ready(function () {
    zNodes = getDepts();
    //左侧组织架构
    $.fn.zTree.init($("#orgnization"), settingOrg, zNodes);
    //编辑选择架构
    $.fn.zTree.init($("#treeDemo"), setting, zNodes);
});

function getDepts() {
    var data = [];
    $.ajax({
        async: false,
        type: 'get',
        dataType: 'json',
        url: '/Department/OrgnizitonItem',
        data: {
        },
        success: function (result) {
            data = result;
        },
        error: function (err) {
            $ses.msg(err.statusText);
        }
    });
    return data;
}