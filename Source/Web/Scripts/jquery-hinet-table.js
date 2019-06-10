var hinet_baseConfig = [];

var hinet_baseElement;
(function ($) {
    $.fn.hinetTable = function (type, options) {
        hinet_baseElement = $(this);

        switch (type) {
            case "init":
                initHinetTable(options);
                break;
            case "data":
                updateHinetTable(options);
                break;
            case "reload":
                reloadHinetTable();
        }
    };


    var initHinetTable = function (options) {

        // Default options
        var settings = $.extend({
            currentPage: 1,
            pageSizeList: { size: [10, 20, 50, -1], label: ['10', '20', '50', 'Tất cả'] },
            showSizePage: true,
            pageSize: 20,
            pagecount: 1,
            recordCount: 0,
            getData: function () { },
            listItem: [],
            config: []
        }, options);


        hinet_baseConfig[hinet_baseElement.selector] = settings;

        /*
        var itemConf = {
            isSort: true,
            nameModel: '',
            content: function (data) {

            }
        }
        */

        //html số lượng bản ghi trên trang
        var strHeader = ' <div style="width:100%; margin-bottom:5px;float:left;">';
        strHeader += '<div style="float:left; padding-right:10px;">';
        strHeader += 'Tổng số: <strong class="red hntbl-counter" >' + settings.recordCount + '</strong>';
        strHeader += '</div>';
        if (settings.showSizePage) {
            strHeader += '<div style="float:right; padding-right:10px; ">';
            strHeader += '<select style="height:25px" class="hntbl-page-size">';
            for (var page = 0; page < settings.pageSizeList.size.length; page++) {
                strHeader += '<option value="' + settings.pageSizeList.size[page] + '">' + settings.pageSizeList.label[page] + '</option>';
            }
            strHeader += '</select>';
            strHeader += '</div>';
        }

        strHeader += '</div>';

        var navigator = '<div style="width:100%">';
        navigator += '<div style="text-align:center;">';
        navigator += '<ul class="pagination pagination-sm navigator hntbl-navigator" style="margin:0px"></ul>';
        navigator += '</div>';
        navigator += '</div>';

        var isTopBar = hinet_baseElement.find("strong.hntbl-counter");

        if (isTopBar.length == 0) {
            if ($(".render-table-info").length > 0) {
                $(".render-table-info").html(strHeader);
            } else {
                hinet_baseElement.prepend(strHeader);
            }
        }
        
        var isfootbar = hinet_baseElement.find("ul.hntbl-navigator");
        if (isfootbar.length == 0) {
            
            hinet_baseElement.append(navigator);
            
        }
        //end

        //gen phân trang
        DefineCssPageList();
        var elementpage = hinet_baseElement.find("ul.navigator");

        GenPaging(1, settings.pagecount, hinet_baseElement.selector);
        //end

        //data generate 
        if (settings.config.length > 0) {
            for (var i = 0; i < settings.config.length; i++) {
                if (settings.config[i].isSort == true) {
                    hinet_baseElement.find(" table.hinet-table thead tr th").eq(i).addClass("isSort");
                }
                if (settings.config[i].nameModel != '') {
                    hinet_baseElement.find(" table.hinet-table thead tr th").eq(i).attr("data-model", settings.config[i].nameModel);
                }
            }
            //fill data
            var strRoot = "";
            var countrow = settings.listItem.length < settings.pageSize ? settings.listItem.length : settings.pageSize;
            for (var item = 0; item < countrow; item++) {
                var data = settings.listItem[item];
                var strContent = "<tr>"
                for (var col = 0; col < settings.config.length; col++) {
                    var tdclass = settings.config[col].tdClass != null ? settings.config[col].tdClass : "";
                    var tdAttr = settings.config[col].tdAttr != null ? settings.config[col].tdAttr : "";
                    if (settings.config[col].isCounter == true) {
                        var stt = item + 1;
                        strContent += "<td class='" + tdclass + "'>" + stt + "</td>";
                    } else {
                        if (settings.config[col].content && typeof (settings.config[col].content) === 'function') {
                            var datacolum = settings.config[col].content(data);
                            var strDatacolum = datacolum != null ? datacolum : "";
                            strContent += "<td " + tdAttr + "='F_" + data.ID + "' class='" + tdclass + "'>" + strDatacolum + "</td>";
                        }
                    }

                }
                strContent += "</tr>";
                strRoot += strContent;
            }
            hinet_baseElement.find("table.hinet-table tbody").html(strRoot);

        }

        //set sort
        hinet_baseElement.find(" table.hinet-table thead tr th.isSort").each(function () {
            $(this).addClass("sort-none");
            $(this).attr("data-sort", "none");
        })
        var divtable = $(this);        
        hinet_baseElement.find(" table.hinet-table thead tr th.isSort").click(function () {
            var trparent = $(this).parent("tr");
            var elementCLick = $(this);
            var sortvalue = elementCLick.attr("data-sort");
            trparent.find("th.isSort").each(function () {
                $(this).removeClass("sort-desc")
                $(this).removeClass("sort-asc")
                $(this).removeClass("sort-none")
                $(this).addClass("sort-none")
                $(this).attr("data-sort", "none");

            })
            elementCLick.attr("data-sort", sortvalue);
            var hinetCover = elementCLick.parents("div.hntbl-cover").first();
            var idDataTable = hinetCover.attr("ID");

            var querySort = "";
            querySort += elementCLick.attr("data-model") + " ";
            switch ($(this).attr("data-sort")) {
                case "none":
                    $(this).removeClass("sort-none");
                    $(this).addClass("sort-desc");
                    $(this).attr("data-sort", "desc");
                    querySort += "desc";
                    break;
                case "desc":
                    $(this).removeClass("sort-none");
                    $(this).removeClass("sort-desc");
                    $(this).addClass("sort-asc");
                    $(this).attr("data-sort", "asc");
                    querySort += "asc";
                    break;
                case "asc":
                    $(this).removeClass("sort-none")
                    $(this).removeClass("sort-asc");
                    $(this).addClass("sort-desc");
                    $(this).attr("data-sort", "desc");
                    querySort += "desc";
                    break;
            }
            var setting = hinet_baseConfig['#' + idDataTable];
            if (setting != null) {
                setting.getData(1, querySort, setting.pageSize);
            }

        })
        if (hinet_baseElement.find(" select.hntbl-page-size").length > 0) {
            hinet_baseElement.find(" select.hntbl-page-size").change(function () {
                var pagesize = $(this).val();
                console.log(pagesize);
                hinet_baseConfig[hinet_baseElement.selector].pageSize = parseInt(pagesize);
                console.log(hinet_baseConfig[hinet_baseElement.selector].pageSize);
                settings.getData(1, "", pagesize);
            });
        }
        if (hinet_baseElement.parent().parent().parent().find(" select.hntbl-page-size").length > 0) {
            hinet_baseElement.parent().parent().parent().find(" select.hntbl-page-size").change(function () {
                var pagesize = $(this).val();
                console.log(pagesize);
                hinet_baseConfig[hinet_baseElement.selector].pageSize = parseInt(pagesize);
                console.log(hinet_baseConfig[hinet_baseElement.selector].pageSize);
                settings.getData(1, "", pagesize);
            });
        }
        
        // Apply options
        return settings;

    };



    var updateHinetTable = function (options) {
        var settings = $.extend({
            pageSize: 20,
            pageIndex: 1,
            pagecount: 1,
            recordCount: 0,
            listItem: [],

        }, options);
        hinet_baseElement.find("strong.hntbl-counter").html(settings.recordCount)
        GenPaging(settings.pageIndex, settings.pagecount, hinet_baseElement.selector);
        hinet_baseConfig[hinet_baseElement.selector].currentPage = settings.pageIndex;
        if (hinet_baseConfig[hinet_baseElement.selector].config) {

            var config = hinet_baseConfig[hinet_baseElement.selector].config;
            var strRoot = "";
            var countrow = settings.listItem.length < settings.pageSize ? settings.listItem.length : settings.pageSize;
            for (var item = 0; item < countrow; item++) {
                var data = settings.listItem[item];
                var strContent = "<tr>"
                for (var col = 0; col < config.length; col++) {
                    var tdclass = config[col].tdClass != null ? config[col].tdClass : "";
                    if (config[col].isCounter == true) {
                        var stt = (settings.pageIndex - 1) * settings.pageSize + item + 1;
                        strContent += "<td class='" + tdclass + "'>" + stt + "</td>";
                    } else {
                        if (config[col].content && typeof (config[col].content) === 'function') {
                            var datacolum = config[col].content(data);
                            var strDatacolum = datacolum != null ? datacolum : "";
                            strContent += "<td class='" + tdclass + "'>" + strDatacolum + "</td>";
                        }
                    }
                }
                strContent += "</tr>";
                strRoot += strContent;
            }
            hinet_baseElement.find("table.hinet-table tbody").html(strRoot);
        }
    }

    //init hinet table function


    //reload table
    var reloadHinetTable = function () {

        var settings = hinet_baseConfig[hinet_baseElement.selector];
        console.log(hinet_baseConfig);
        if (settings != null) {
            settings.getData(settings.currentPage, "", settings.pageSize);
        }

    }

}(jQuery));

function ActionPaging(id, total, updateID) {
    var config = hinet_baseConfig[updateID];
    if (id <= 0) {
        id = 1;
    }
    config.getData(id, "", config.pageSize);
    GenPaging(id, total, updateID);
}



function GenPaging(index, total, targetID) {
    var strPage = "";
    strPage += '<li><a href="javascript:ActionPaging(' + 1 + ',' + total + ',\'' + targetID + '\');">Trang đầu</a></li>';
    if (index > 3) {
        strPage += '<li class="disabled"><a href="javascript:void(0);">...</a></li>';
    }
    for (var i = -3; i <= 3; i++) {
        var page = i + index;
        if (i == 0) {
            strPage += '<li class="active"><a href="javascript:void(0)">' + page + '</a></li>';
        } else {
            if (page > 0 && page <= total) {
                strPage += '<li><a href="javascript:ActionPaging(' + page + ',' + total + ',\'' + targetID + '\');">' + page + '</a></li>';
            }

        }
    }
    if (index + 3 < total) {
        strPage += '<li class="disabled"><a href="javascript:void(0);">...</a></li>';
    }
    strPage += '<li><a href="javascript:ActionPaging(' + total + ',' + total + ',\'' + targetID + '\');">Trang cuối</a></li>';


    $(targetID + " ul.navigator").html(strPage);

}

