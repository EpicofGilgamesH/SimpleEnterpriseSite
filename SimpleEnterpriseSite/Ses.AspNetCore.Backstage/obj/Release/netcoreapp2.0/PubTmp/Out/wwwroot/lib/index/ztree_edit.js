var setting = {
    view: {
        selectedMulti: false
    },
    edit: {
        enable: true,
        showRemoveBtn: false,
        showRenameBtn: false
    },
    data: {
        keep: {
            parent: true,
            leaf: true
        },
        simpleData: {
            enable: true
        }
    },
    callback: {
        beforeDrag: beforeDrag,
        beforeRemove: beforeRemove,
        beforeRename: beforeRename,
        onRemove: onRemove,
        onClick: onSelectClick
    }
};


var zNodes = [];
var currentId = '';
function getOrgnizition() {
    $.ajax({
        type: 'GET',
        async: false,
        dataType: 'json',
        data: {},
        url: '/Department/OrgnizitonItem',
        success: function (result) {
            for (var i = 0; i < result.length; i++) {
                zNodes[i] = result[i];
            }
        },
        error: function (error) {
            layer.msg(error);
        }
    });
};

var log, className = "dark";
function beforeDrag(treeId, treeNodes) {
    return false;
}
function beforeRemove(treeId, treeNode) {
    //className = (className === "dark" ? "" : "dark");
    //showLog("[ " + getTime() + " beforeRemove ]&nbsp;&nbsp;&nbsp;&nbsp; " + treeNode.name);
    //layer.confirm("确认删除--" + treeNode.name + "吗？", {
    //    btn: ['确认', '取消']
    //}, function () {

    //}, function () {
    //    layer.closeAll('dialog');
    //    return false;
    //});

    return confirm("确认删除 节点 -- " + treeNode.name + " 吗？");
}
function onRemove(e, treeId, treeNode) {
    showLog("[ " + getTime() + " onRemove ]&nbsp;&nbsp;&nbsp;&nbsp; " + treeNode.name);
}
function beforeRename(treeId, treeNode, newName) {
    if (newName.length == 0) {
        layer.msg("节点名称不能为空.");
        var zTree = $.fn.zTree.getZTreeObj("treeDemo");
        setTimeout(function () { zTree.editName(treeNode) }, 10);
        return false;
    }
    //操作数据库
    if (operate == 1) {
        var id = addDept(treeNode.pId, newName, treeNode.isParent);
        treeNode.id = currentId;
    } else if (operate == 2) {
        editDept(treeNode.id, newName);
    }
    return true;
}
//function showLog(str) {
//    if (!log) log = $("#log");
//    log.append("<li class='" + className + "'>" + str + "</li>");
//    if (log.children("li").length > 8) {
//        log.get(0).removeChild(log.children("li")[0]);
//    }
//}
//function getTime() {
//    var now = new Date(),
//        h = now.getHours(),
//        m = now.getMinutes(),
//        s = now.getSeconds(),
//        ms = now.getMilliseconds();
//    return (h + ":" + m + ":" + s + " " + ms);
//}

var newCount = 1;
var operate = 0;  //1,新增节点 2,修改名称
function add(e) {
    var zTree = $.fn.zTree.getZTreeObj("treeDemo"),
        isParent = e.data.isParent,
        nodes = zTree.getSelectedNodes(),
        treeNode = nodes[0];
    if (treeNode) {
        treeNode = zTree.addNodes(treeNode, { id: (100 + newCount), pId: treeNode.id, isParent: isParent, name: "new node" + (newCount++) });
    } else {
        treeNode = zTree.addNodes(null, { id: (100 + newCount), pId: 0, isParent: isParent, name: "new node" + (newCount++) });
    }
    if (treeNode) {
        zTree.editName(treeNode[0]);
        operate = 1;
    } else {
        layer.msg("最底层节点无法新增子节点");
    }
};
function edit() {
    var zTree = $.fn.zTree.getZTreeObj("treeDemo"),
        nodes = zTree.getSelectedNodes(),
        treeNode = nodes[0];
    if (nodes.length == 0) {
        layer.msg("请先选择一个节点");
        return;
    }
    zTree.editName(treeNode);
    operate = 2;
};
function remove(e) {
    var zTree = $.fn.zTree.getZTreeObj("treeDemo"),
        nodes = zTree.getSelectedNodes(),
        treeNode = nodes[0];
    if (nodes.length == 0) {
        layer.msg("请先选择一个节点");
        return;
    }
    //var callbackFlag = $("#callbackTrigger").attr("checked");
    layer.confirm("确认删除--" + treeNode.name + "吗？", {
        btn: ['确认', '取消']
    }, function () {
        zTree.removeNode(treeNode, false);
        deleteDept(treeNode.id);
        layer.closeAll('dialog');
    }, function () {
        layer.closeAll('dialog');
    });
};
//清空子节点
function clearChildren(e) {
    var zTree = $.fn.zTree.getZTreeObj("treeDemo"),
        nodes = zTree.getSelectedNodes(),
        treeNode = nodes[0];
    if (nodes.length == 0 || !nodes[0].isParent) {
        layer.msg("请先选择一个父节点");
        return;
    }
    zTree.removeChildNodes(treeNode);
};

$(document).ready(function () {
    refreshTree();
});

//刷新树结构
function refreshTree() {
    getOrgnizition();
    $.fn.zTree.init($("#treeDemo"), setting, zNodes);
    $("#addParent").bind("click", { isParent: true }, add);
    $("#addLeaf").bind("click", { isParent: false }, add);
    $("#edit").bind("click", edit);
    $("#remove").bind("click", remove);
    $("#clearChildren").bind("click", clearChildren);
}

//数据库操作 Node: pid,name
function addDept(pid, name, isParent) {
    debugger;
    //$ace.post("/Department/AddDept", data, function (result) {
    //    $ace.msg(result.Message);
    //    currentId = result.Data;
    //    //setTimeout(refresh, 2000);//刷新table
    //});

    //需要ajax同步加载
    $.ajax({
        type: 'GET',
        async: false,
        dataType: 'json',
        data: {
            pid: pid,
            name: name,
            isParent: isParent
        },
        url: '/Department/AddDept',
        success: function (result) {
            currentId = result.Data;
            layer.msg(result.Message);
        },
        error: function (error) {
            layer.msg(error);
        }
    });
}

function editDept(id, name) {
    var data = {
        id: id,
        name: name
    }
    $ace.post("/Department/EditDept", data, function (result) {
        $ace.msg(result.Message);
        setTimeout(refresh, 1000);//刷新table
    });
}

function deleteDept(id) {
    var data = {
        id: id,
    }
    $ace.post("/Department/DeleteDept", data, function (result) {
        $ace.msg(result.Message);
        setTimeout(refresh, 1000);//刷新table
        refreshTree();
    });
}

//组织树点击事件
var detpSearchId = '';
function onSelectClick(e, treeId, treeNode) {
    var zTree = $.fn.zTree.getZTreeObj("treeDemo"),
        nodes = zTree.getSelectedNodes(),
        v = "";
    if (nodes != null)
        v = nodes[0].id;
    detpSearchId = v;
    $("#tb_Roles").bootstrapTable('refresh');
}


function refresh() {
    $("#tb_Roles").bootstrapTable('refresh');
}

