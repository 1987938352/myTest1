

(function ($, window) {
    var applicationPath = window.applicationPath === "" ? "" : window.applicationPath || "../..";
    var UpdataLoadarrayObj = new Array();
    function SuiJiNum() {
        return (((1 + Math.random()) * 0x10000) | 0).toString(16).substring(1);
    }

    function initWebUpload(item, options) {

        if (!WebUploader.Uploader.support()) {
            var error = "上传控件不支持您的浏览器！请尝试升级flash版本或者使用Chrome引擎的浏览器。<a target='_blank' href='http://se.360.cn'>下载页面</a>";
            if (window.console) {
                window.console.log(error);
            }
            $(item).text(error);
            return;
        }
        var target = $(item);//容器
        if (target.find(".uploader-list").length > 0) {
            return;
        }
        //创建默认参数
        var defaults = {
            auto: true,
            hiddenInputId: "uploadifyHiddenInputId", // input hidden id
            onAllComplete: function (event) { }, // 当所有file都上传后执行的回调函数
            onComplete: function (event) { },// 每上传一个file的回调函数
            fileNumLimit: undefined,//验证文件总数量, 超出则不允许加入队列
            fileSizeLimit: undefined,//验证文件总大小是否超出限制, 超出则不允许加入队列。
            fileSingleSizeLimit: undefined,//验证单个文件大小是否超出限制, 超出则不允许加入队列
            PostbackHold: false
        };
        var opts = $.extend(defaults, options);
        var hdFileData = $("#" + opts.hiddenInputId);
        var pickerid = "";
        if (typeof guidGenerator36 != 'undefined')//给一个唯一ID
            pickerid = guidGenerator36();
        else
            pickerid = (((1 + Math.random()) * 0x10000) | 0).toString(16).substring(1);
        var uploaderStrdiv = '<div class="webuploader">'
        if (opts.auto) {
            uploaderStrdiv =
                '<div  class="uploader-list"></div>' +
                '<div class="btns">' +
                '<div id="' + pickerid + '">选择文件</div>' +
                '</div>'

        } else {
            uploaderStrdiv =
                '<div  class="uploader-list"></div>' +
                '<div class="btns">' +
                '<div id="' + pickerid + '">选择文件</div>' +
                '<button class="webuploadbtn">开始上传</button>' +
                '</div>'
        }
        uploaderStrdiv += '<div style="display:none" class="UploadhiddenInput" >\
                         </div>'
        uploaderStrdiv += '</div>';
        target.append(uploaderStrdiv);

        var $list = target.find('.uploader-list'),
            $btn = target.find('.webuploadbtn'),//手动上传按钮备用
            state = 'pending',
            $hiddenInput = target.find('.UploadhiddenInput')
        var jsonData = {
            fileList: []
        };
        debugger;
        var webuploaderoptions = $.extend({

            // swf文件路径
            swf: applicationPath + '/webuploader-0.1.5/Uploader.swf',
            // 文件接收服务端。
            server: '/User/UploadPic',
            deleteServer: '/User/DeleteFile',
            // 选择文件的按钮。可选。
            // 内部根据当前运行是创建，可能是input元素，也可能是flash.
            pick: '#' + pickerid,
            //不压缩image, 默认如果是jpeg，文件上传前会压缩一把再上传！
            resize: false,
            runtimeOrder: 'flash',
            fileNumLimit: opts.fileNumLimit,
            fileSizeLimit: opts.fileSizeLimit,
            fileSingleSizeLimit: opts.fileSingleSizeLimit,
            //限制只能上传图片,格式gif,jpg,jpeg,bmp,png
            accept: {
                title: 'Images',
                extensions: 'gif,jpg,jpeg,bmp,png',
                mimeTypes: 'image/*'
            }
        },
            opts);
        var uploader = WebUploader.create(webuploaderoptions);
        UpdataLoadarrayObj[$(item)[0].id] = uploader;
        var ratio = window.devicePixelRatio || 1

        // 缩略图大小
        var thumbnailWidth = 110 * ratio
        var thumbnailHeight = 110 * ratio
        if (opts.auto) {
            // 优化retina, 在retina下这个值是2

            uploader.on('fileQueued', function (file) {

                var $li = $('<div id="' + $(item)[0].id + file.id + '" class="file-item thumbnail">' +
                    '<img>' +
                    '<div class="info">' + file.name + '</div>' +
                    '</div>');

                $img = $li.find('img');
                uploader.makeThumb(file, function (error, src) {
                    if (error) {
                        $img.replaceWith('<span>不能预览</span>');
                        return;
                    }

                    $img.attr('src', src);
                }, thumbnailWidth, thumbnailHeight);

                // $list为容器jQuery实例
                $list.append($li);

                $btns = $('<div class="file-panel">' +
                    '<span class="cancel">删除</span>').appendTo($li)
                uploader.upload();
            });
        } else {
            uploader.on('fileQueued', function (file) {//队列事件

                var $li = $('<div id="' + $(item)[0].id + file.id + '" class="file-item thumbnail">' +
                    '<img>' +
                    '<div class="info">' + file.name + '</div>' +
                    '</div>');

                $img = $li.find('img');
                uploader.makeThumb(file, function (error, src) {
                    if (error) {
                        $img.replaceWith('<span>不能预览</span>');
                        return;
                    }

                    $img.attr('src', src);
                }, thumbnailWidth, thumbnailHeight);

                // $list为容器jQuery实例
                $list.append($li);

                $btns = $('<div class="file-panel">' +
                    '<span class="cancel">删除</span>').appendTo($li)
            });
        }
        // 文件上传过程中创建进度条实时显示。
        uploader.on('uploadProgress', function (file, percentage) {
            var $li = $('#' + $(item)[0].id + file.id),
                $percent = $li.find('.progress span');

            // 避免重复创建
            if (!$percent.length) {
                $percent = $('<p class="progress"><span></span></p>')
                    .appendTo($li)
                    .find('span');
            }

            $percent.css('width', percentage * 100 + '%');
        });

        // 文件上传成功，给item添加成功class, 用样式标记上传成功。
        uploader.on('uploadSuccess', function (file, response) {
            $('#' + $(item)[0].id + file.id).addClass('upload-state-done');
            var $li = $('#' + $(item)[0].id + file.id),
                $error = $li.find('div.error');

            // 避免重复创建
            if (!$error.length) {
                $error = $('<div class="success"></div>').appendTo($li);
            }
            if (response.state == "error") {
                $error.text(response.message);
            } else {
                $error.text('上传完成');
                $hiddenInput.append('<input type="text" id="hiddenInput' + $(item)[0].id + file.id + '" class="hiddenInput" value="' + response.message + '" />')
            }

        });

        // 文件上传失败，显示上传出错。
        uploader.on('uploadError', function (file) {
            var $li = $('#' + $(item)[0].id + file.id),
                $error = $li.find('div.error');

            // 避免重复创建
            if (!$error.length) {
                $error = $('<div class="error"></div>').appendTo($li);
            }

            $error.text('上传失败');
        });

        // 完成上传完了，成功或者失败，先删除进度条。
        uploader.on('uploadComplete', function (file, response) {
            $('#' + $(item)[0].id + file.id).find('.progress').remove();
        });

        //uploader.on('uploadProgress', function (file, percentage) {//进度条事件
        //    var $li = target.find('#' + $(item)[0].id + file.id),
        //        $percent = $li.find('.progress .bar');

        //    // 避免重复创建
        //    if (!$percent.length) {
        //        $percent = $('<span class="progress">' +
        //            '<span  class="percentage"><span class="text"></span>' +
        //          '<span class="bar" role="progressbar" style="width: 0%">' +
        //          '</span></span>' +
        //        '</span>').appendTo($li).find('.bar');
        //    }

        //    $li.find('span.webuploadstate').html('上传中');
        //    $li.find(".text").text(Math.round(percentage * 100) + '%');
        //    $percent.css('width', percentage * 100 + '%');
        //});
        //uploader.on('uploadSuccess', function (file, response) {//上传成功事件
        //    if (response.state == "error") {
        //        target.find('#' + $(item)[0].id + file.id).find('span.webuploadstate').html(response.message);
        //    } else {
        //        target.find('#' + $(item)[0].id + file.id).find('span.webuploadstate').html('已上传');
        //        $hiddenInput.append('<input type="text" id="hiddenInput' + $(item)[0].id + file.id + '" class="hiddenInput" value="' + response.message + '" />')
        //    }
        //});

        //uploader.on('uploadError', function (file) {
        //    target.find('#' + $(item)[0].id + file.id).find('span.webuploadstate').html('上传出错');
        //});

        //uploader.on('uploadComplete', function (file) {//全部完成事件
        //    target.find('#' + $(item)[0].id + file.id).find('.progress').fadeOut();
        //});



        uploader.on('all', function (type) {
            if (type === 'startUpload') {
                state = 'uploading';
            } else if (type === 'stopUpload') {
                state = 'paused';
            } else if (type === 'uploadFinished') {
                state = 'done';
            }

            if (state === 'uploading') {
                $btn.text('暂停上传');
            } else {
                $btn.text('开始上传');
            }
        });

        //删除时执行的方法
        uploader.on('fileDequeued', function (file) {
            debugger;
            var fullName = $("#hiddenInput" + $(item)[0].id + file.id).val();
            if (fullName != null) {
                $.post(webuploaderoptions.deleteServer, { fullName: fullName }, function (data) {
                    // alert(data.message);
                })
            }
            $("#" + $(item)[0].id + file.id).remove();
            $("#hiddenInput" + $(item)[0].id + file.id).remove();

        })

        //多文件点击上传的方法
        $btn.on('click', function () {
            if (state === 'uploading') {
                uploader.stop();
            } else {
                uploader.upload();
            }
        });

        //删除
        $list.on("click", ".file-panel", function () {
            debugger
            var $ele = $(this);
            var id = $ele.parent().attr("id");
            var id = id.replace($(item)[0].id, "");

            var file = uploader.getFile(id);
            uploader.removeFile(file);
        });

    }
    $.fn.CleanUpload = function (options) {
        var uploadrFile = UpdataLoadarrayObj[$(this).attr("id")]
        var fileslist = uploadrFile.getFiles();
        for (var i in fileslist) {
            uploadrFile.removeFile(fileslist[i]);
        }
        //var ele = $(this);
        //var filesdata = ele.find(".UploadhiddenInput");
        //filesdata.find(".hiddenInput").remove();
        //ele.find(".uploader-list .item").remove();
    }
    $.fn.GetFilesAddress = function (options) {
        var ele = $(this);
        var filesdata = ele.find(".UploadhiddenInput");
        var filesAddress = [];
        filesdata.find(".hiddenInput").each(function () {
            filesAddress.push($(this).val());
        })
        return filesAddress;

    }

    $.fn.powerWebUpload = function (options) {
        var ele = this;

        if (typeof WebUploader == 'undefined') {
            var casspath = applicationPath + "/webuploader-0.1.5/webuploader.css";
            $("<link>").attr({ rel: "stylesheet", type: "text/css", href: casspath }).appendTo("head");
            var jspath = applicationPath + "/webuploader-0.1.5/webuploader.min.js";
            $.getScript(jspath).done(function () {

                initWebUpload(ele, options);
            })
                 .fail(function () {
                    alert("请检查webuploader的路径是否正确!")
                });

        }
        else {
            initWebUpload(ele, options);
        }
    }
})(jQuery, window);