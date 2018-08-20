function pemission(peId, btn) {
    this.peId = peId;
    this.btn = btn;
}
var plist = [];
var dynamicId = '';
var typeId = '';

//保存 权限提交
//function savePermission() {
//    var treeObj = $.fn.zTree.getZTreeObj("treePermission");
//    var nodes = treeObj.getCheckedNodes(true);
//    for (var i = 0; i < nodes.length; i++) {
//        var checkBoxValue = "";
//        if (nodes[i].isParent) continue;
//        //pemission.btn = element.myAttr;
//        var test = $("input[name='check_" + nodes[i].name + "']:checked");
//        test.each(function () {
//            checkBoxValue += $(this).val() + ",";
//        });
//        var pemissions = new pemission(nodes[i].id, checkBoxValue);
//        plist.push(pemissions);
//    }
//    var data = {
//        dynamic: dynamicId,
//        type: type,
//        pemission: plist,
//    }
//    $ace.post('/Permission/OperateSubmit', data, function (data) {
//        $ace.msg(data.Message);
//        layer.close(index);
//        plist = [];
//    });
//}

$("#btnpermission").click(function () {
    debugger;
    plist = [];
    var treeObj = $.fn.zTree.getZTreeObj("treePermission");
    var nodes = treeObj.getCheckedNodes(true);
    for (var i = 0; i < nodes.length; i++) {
        var checkBoxValue = "";
        //if (nodes[i].isParent) continue;
        //pemission.btn = element.myAttr;
        var test = $("input[name='check_" + nodes[i].name + "']:checked");
        test.each(function () {
            checkBoxValue += $(this).val() + ",";
        });
        var pemissions = new pemission(nodes[i].id, checkBoxValue);
        plist.push(pemissions);
    }
    var data = {
        dynamic: dynamicId,
        type: typeId,
        pemission: plist,
    }
    $ace.post('/Permission/OperateSubmit', data, function (data) {
        $ace.msg(data.Message);
        layer.close(index);
        plist = [];
    });
});
//取消 事件
$("#closepermission").click(function () {
    layer.close(index);
});


var index = 0;
//弹窗事件
function openLayer() {
    index = layer.open({
        type: 1,
        title: '权限',
        skin: 'layui-layer-rim', //样式类名
        area: ['800px', '600px'], //宽高
        closeBtn: 0, //不显示关闭按钮
        anim: 2,
        shadeClose: true, //开启遮罩关闭
        content: $("#Permission"),
    });
};


//权限树
var setting1 = {
    check: {
        enable: true
    },
    data: {
        simpleData: {
            enable: true
        }
    },
    view: {
        addDiyDom: addDiyDom
    }
};
var IDMark_A = "_a";
var zNodes1 = [];

function setCheck() {
    var zTree = $.fn.zTree.getZTreeObj("treePermission"),
        type = { "Y": "ps", "N": "ps" };
    zTree.setting.check.chkboxType = type;
}

//初始化权限数据
function initpermissiontree(dynamic, type) {
    dynamicId = dynamic;
    typeId = type;
    getzNodes(dynamic, type);
    $.fn.zTree.init($("#treePermission"), setting1, zNodes1);
    setCheck();
}

function getzNodes() {
    $.ajax({
        async: false,
        type: 'get',
        dataType: 'json',
        url: '/Permission/PermissionTreeJson',
        data: {
            dynamic: dynamicId,
            type: typeId
        },
        success: function (result) {
            zNodes1 = result;
        },
        error: function (err) {
            $ace.msg(err.statusText);
        }
    })
}

//给权限树添加按钮控制
function addDiyDom(treeId, treeNode) {
    if (treeNode.isParent) { return; }
    //获取按钮权限
    var btnpsm = treeNode.myAttr;
    var ischecked = "checked";
    var unchecked = "";
    var addstyle = unchecked, editstyle = unchecked, deletestyle = unchecked, uploadstyle = unchecked, downloadstyle = unchecked;
    if (typeof (btnpsm) != "undefined" && btnpsm != null) {
        if (btnpsm.indexOf(btnPsm.addBtn) != -1)
            addstyle = ischecked;
        if (btnpsm.indexOf(btnPsm.editBtn) != -1)
            editstyle = ischecked;
        if (btnpsm.indexOf(btnPsm.deleteBtn) != -1)
            deletestyle = ischecked;
        if (btnpsm.indexOf(btnPsm.uploadBtn) != -1)
            uploadstyle = ischecked;
        if (btnpsm.indexOf(btnPsm.downloadBtn) != -1)
            downloadstyle = ischecked;
    }
    var aObj = $("#" + treeNode.tId + IDMark_A);
    //添加图标
    //var addBtnIcon = "<label class='demoIcon'  id='addBtn_" + treeNode.id + "' title='" + treeNode.name + "' onfocus='this.blur();'><input type='checkbox' id='addBtn_" + treeNode.id + " ><i class='fa fa-plus-circle'" + addstyle + "></i></label>";
    var addBtnIcon = "&nbsp;<input type='checkbox' value='" + btnPsm.addBtn + "' name='check_" + treeNode.name + "' " + addstyle + "> 增  ";
    aObj.after(addBtnIcon);

    //var addBtn = $("#addBtn_" + treeNode);
    //if (addBtn) {
    //    addBtn.bind("click", function () {

    //    })
    //}
    //编辑图标
    //var editBtnIcon = "<span class='demoIcon' id='editBtn_" + treeNode.id + "' title='" + treeNode.name + "' onfocus='this.blur();'><i class='fa fa-edit' " + addstyle + "></i></span>";
    var editBtnIcon = "&nbsp;<input type='checkbox' value='" + btnPsm.editBtn + "' name='check_" + treeNode.name + "' " + editstyle + ">改  ";
    aObj.after(editBtnIcon);
    //var editBtn = $("#editBtn_" + treeNode);
    //if (editBtn) { editBtn.bind("click", function () { }) }
    //删除图标
    //var deleteBtnIcon = "<span class='demoIcon' id='deleteBtn_" + treeNode.id + "' title='" + treeNode.name + "' onfocus='this.blur();'><i class='fa fa-times-circle' " + addstyle + "></i></span>";
    var deleteBtnIcon = "&nbsp;<input type='checkbox' value='" + btnPsm.deleteBtn + "' name='check_" + treeNode.name + "' " + deletestyle + "> 删  ";
    aObj.after(deleteBtnIcon);
    //var deleteBtn = $("#deleteBtn_" + treeNode);
    //if (deleteBtn) { deleteBtn.bind("click", function () { }) }
    //上传图标
    //var uploadBtnIcon = "<span class='demoIcon' id='uploadBtn_" + treeNode.id + "' title='" + treeNode.name + "' onfocus='this.blur();'><i class='fa fa-upload' " + addstyle + "></i></span>";
    var uploadBtnIcon = "&nbsp;<input type='checkbox' value='" + btnPsm.uploadBtn + "' name='check_" + treeNode.name + "' " + uploadstyle + "> 上传  ";
    aObj.after(uploadBtnIcon);
    //var uploadBtn = $("#uploadBtn_" + treeNode);
    //if (uploadBtn) { uploadBtn.bind("click", function () { }) }
    //下载图标
    //var downloadBtnIcon = "<span class='demoIcon' id='downloadBtn_" + treeNode.id + "' title='" + treeNode.name + "' onfocus='this.blur();'><i class='fa fa-download' " + addstyle + "></i></span>";
    var downloadBtnIcon = "&nbsp;&nbsp;&nbsp;&nbsp;<input type='checkbox' value='" + btnPsm.downloadBtn + "' name='check_" + treeNode.name + "' " + downloadstyle + "> 下载  ";
    aObj.after(downloadBtnIcon);
    //var downloadBtn = $("#downloadBtn_" + treeNode);
    //if (downloadBtn) { downloadBtn.bind("click", function () { }) }
}