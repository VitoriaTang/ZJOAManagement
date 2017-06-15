/*
jQWidgets v4.5.3 (2017-June)
Copyright (c) 2011-2017 jQWidgets.
License: http://jqwidgets.com/license/
*/
!function(a){"use strict";a.jqx.jqxWidget("jqxFileUpload","",{}),a.extend(a.jqx._jqxFileUpload.prototype,{defineInstance:function(){var b={width:null,height:"auto",uploadUrl:"",fileInputName:"",autoUpload:!1,multipleFilesUpload:!0,accept:null,browseTemplate:"",uploadTemplate:"",cancelTemplate:"",localization:null,renderFiles:null,disabled:!1,rtl:!1,events:["select","remove","uploadStart","uploadEnd"]};return this===a.jqx._jqxFileUpload.prototype?b:(a.extend(!0,this,b),b)},createInstance:function(){var b=this;if(void 0===b.host.jqxButton)throw new Error("jqxFileUpload: Missing reference to jqxbuttons.js");b._createFromInput("jqxFileUpload"),a.jqx.browser.msie?a.jqx.browser.version<11&&(b._ieOldWebkit=!0,a.jqx.browser.version<8&&(b._ie7=!0)):a.jqx.browser.webkit&&(b._ieOldWebkit=!0),b._fluidWidth="string"==typeof b.width&&"%"===b.width.charAt(b.width.length-1),b._fluidHeight="string"==typeof b.height&&"%"===b.height.charAt(b.height.length-1),b._render(!0)},_createFromInput:function(b){var c=this;if("input"==c.element.nodeName.toLowerCase()){c.field=c.element,c.field.className&&(c._className=c.field.className);var d={title:c.field.title};c.field.id.length?d.id=c.field.id.replace(/[^\w]/g,"_")+"_"+b:d.id=a.jqx.utilities.createId()+"_"+b;var e=a("<div></div>",d);e[0].style.cssText=c.field.style.cssText,c.width||(c.width=a(c.field).width()),c.height||(c.height=a(c.field).outerHeight()),a(c.field).hide().after(e);var f=c.host.data();if(c.host=e,c.host.data(f),c.element=e[0],c.element.id=c.field.id,c.field.id=d.id,c._className&&(c.host.addClass(c._className),a(c.field).removeClass(c._className)),c.field.tabIndex){var g=c.field.tabIndex;c.field.tabIndex=-1,c.element.tabIndex=g}}},_render:function(b){var c=this;c._setSize(),c._addClasses(),!0===b?c._appendElements():c._removeHandlers(),c._addHandlers(),c._ie7&&(c._borderAndPadding("width",c.host),"auto"!==c.height&&c._borderAndPadding("height",c.host)),a.jqx.utilities.resize(c.host,null,!0),a.jqx.utilities.resize(c.host,function(){if(c._fluidWidth){c._ie7&&(c.host.css("width",c.width),c._borderAndPadding("width",c.host));for(var a=0;a<c._fileRows.length;a++){var b=c._fileRows[a],d=b.fileRow;c._ie7&&(d.css("width","100%"),c._borderAndPadding("width",d)),c.renderFiles||c._setMaxWidth(b)}if(c.rtl&&c._ieOldWebkit)for(var e=0;e<c._forms.length;e++){var f=c._browseButton.position();c._forms[e].form.css({left:f.left,top:f.top})}}c._ie7&&c._fluidHeight&&(c.host.css("height",c.height),c._borderAndPadding("height",c.host))})},render:function(){this._render(!1)},refresh:function(a){!0!==a&&this._render(!1)},destroy:function(){var a=this;a.cancelAll(),a._removeHandlers(!0),a.host.remove()},browse:function(){if(!(a.jqx.browser.msie&&a.jqx.browser.version<10)){var b=this;(!0===b.multipleFilesUpload||!1===b.multipleFilesUpload&&0===b._fileRows.length)&&b._forms[b._forms.length-1].fileInput.click()}},_uploadFile:function(a){var b=this;0===b._uploadQueue.length&&b._uploadQueue.push(a),b.renderFiles||(a.uploadFile.add(a.cancelFile).hide(),a.loadingElement.show()),a.fileInput.attr("name",b.fileInputName),b._raiseEvent("2",{file:a.fileName}),a.form[0].submit(),b._fileObjectToRemove=a},uploadFile:function(a){var b=this,c=b._fileRows[a];void 0!==c&&b._uploadFile(c)},uploadAll:function(){var a=this;if(a._fileRows.length>0){for(var b=a._fileRows.length-1;b>=0;b--)a._uploadQueue.push(a._fileRows[b]);a._uploadFile(a._fileRows[0])}},cancelFile:function(a){var b=this;b._removeSingleFileRow(b._fileRows[a])},cancelAll:function(){var a=this;if(a._fileRows.length>0){for(var b=0;b<a._fileRows.length;b++)a._removeFileRow(a._fileRows[b]);setTimeout(function(){a._browseButton.css("margin-bottom",0)},400),a._fileRows.length=0,a._hideButtons(!0)}},propertyChangedHandler:function(b,c,d,e){var f=b.element.id;if("localization"===c)return!e.browseButton||d&&e.browseButton===d.browseButton||(b._browseButton.text(e.browseButton),b._browseButton.jqxButton({width:"auto"})),!e.uploadButton||d&&e.uploadButton===d.uploadButton||(b._uploadButton.text(e.uploadButton),b._uploadButton.jqxButton({width:"auto"})),!e.cancelButton||d&&e.cancelButton===d.cancelButton||(b._cancelButton.text(e.cancelButton),b._cancelButton.jqxButton({width:"auto"})),void(b.renderFiles||(!e.uploadFileTooltip||d&&e.uploadFileTooltip===d.uploadFileTooltip||a("#"+f+" .jqx-file-upload-file-upload").attr("title",e.uploadFileTooltip),!e.uploadFileTooltip||d&&e.cancelFileTooltip===d.cancelFileTooltip||a("#"+f+" .jqx-file-upload-file-cancel").attr("title",e.cancelFileTooltip)));if(e!==d)switch(c){case"width":if(b.host.css("width",e),b._ie7){b._borderAndPadding("width",b.host);for(var g=0;g<b._fileRows.length;g++){var h=b._fileRows[g].fileRow;h.css("width","100%"),b._borderAndPadding("width",h)}}return void(b._fluidWidth="string"==typeof e&&"%"===e.charAt(e.length-1));case"height":return b.host.css("height",e),b._ie7&&b._borderAndPadding("height",b.host),void(b._fluidHeight="string"==typeof e&&"%"===e.charAt(e-1));case"uploadUrl":for(var i=0;i<b._forms.length;i++)b._forms[i].form.attr("action",e);return;case"accept":for(var j=0;j<b._forms.length;j++)b._forms[j].fileInput.attr("accept",e);return;case"theme":return a.jqx.utilities.setTheme(d,e,b.host),b._browseButton.jqxButton({theme:e}),b._uploadButton.jqxButton({theme:e}),void b._cancelButton.jqxButton({theme:e});case"browseTemplate":return void b._browseButton.jqxButton({template:e});case"uploadTemplate":return void b._uploadButton.jqxButton({template:e});case"cancelTemplate":return void b._cancelButton.jqxButton({template:e});case"disabled":return b._browseButton.jqxButton({disabled:e}),b._uploadButton.jqxButton({disabled:e}),b._cancelButton.jqxButton({disabled:e}),void(!0===e?b.host.addClass(b.toThemeProperty("jqx-fill-state-disabled")):b.host.removeClass(b.toThemeProperty("jqx-fill-state-disabled")));case"rtl":;return void function(c){var d=c?"addClass":"removeClass";b._browseButton[d](b.toThemeProperty("jqx-file-upload-button-browse-rtl")),b._cancelButton[d](b.toThemeProperty("jqx-file-upload-button-cancel-rtl")),b._uploadButton[d](b.toThemeProperty("jqx-file-upload-button-upload-rtl")),a.jqx.browser.msie&&a.jqx.browser.version>8&&b._uploadButton[d](b.toThemeProperty("jqx-file-upload-button-upload-rtl-ie"));for(var e=0;e<b._fileRows.length;e++){var f=b._fileRows[e];f.fileNameContainer[d](b.toThemeProperty("jqx-file-upload-file-name-rtl")),f.cancelFile[d](b.toThemeProperty("jqx-file-upload-file-cancel-rtl")),f.uploadFile[d](b.toThemeProperty("jqx-file-upload-file-upload-rtl")),f.loadingElement[d](b.toThemeProperty("jqx-file-upload-loading-element-rtl"))}}(e)}},_raiseEvent:function(b,c){void 0===c&&(c={owner:null});var d=this.events[b];c.owner=this;var e=new a.Event(d);return e.owner=this,e.args=c,e.preventDefault&&e.preventDefault(),this.host.trigger(e)},_setSize:function(){var a=this;a.host.css("width",a.width),a.host.css("height",a.height)},_borderAndPadding:function(a,b){var c;c="width"===a?parseInt(b.css("border-left-width"),10)+parseInt(b.css("border-right-width"),10)+parseInt(b.css("padding-left"),10)+parseInt(b.css("padding-right"),10):parseInt(b.css("border-top-width"),10)+parseInt(b.css("border-bottom-width"),10)+parseInt(b.css("padding-top"),10)+parseInt(b.css("padding-bottom"),10),b.css(a,b[a]()-c)},_addClasses:function(){var a=this;a.host.addClass(a.toThemeProperty("jqx-widget jqx-widget-content jqx-rc-all jqx-file-upload")),!0===a.disabled&&a.host.addClass(a.toThemeProperty("jqx-fill-state-disabled"))},_appendElements:function(){var b=this,c="Browse",d=90,e="Upload All",f=90,g="Cancel All",h=90,i=b.element.id;b.localization&&(b.localization.browseButton&&(c=b.localization.browseButton,d="auto"),b.localization.uploadButton&&(e=b.localization.uploadButton,f="auto"),b.localization.cancelButton&&(g=b.localization.cancelButton,h="auto")),b._browseButton=a('<button id="'+i+'BrowseButton" class="'+b.toThemeProperty("jqx-file-upload-button-browse")+'">'+c+"</button>"),b.host.append(b._browseButton),b._browseButton.jqxButton({theme:b.theme,width:d,template:b.browseTemplate,disabled:b.disabled}),b._browseButton.after('<div style="clear: both;"></div>'),b._bottomButtonsContainer=a('<div class="'+b.toThemeProperty("jqx-file-upload-buttons-container")+'"></div>'),b.host.append(b._bottomButtonsContainer),b._uploadButton=a('<button id="'+i+'UploadButton" class="'+b.toThemeProperty("jqx-file-upload-button-upload")+'">'+e+"</button>"),b._bottomButtonsContainer.append(b._uploadButton),b._uploadButton.jqxButton({theme:b.theme,width:f,template:b.uploadTemplate,disabled:b.disabled}),b._cancelButton=a('<button id="'+i+'CancelButton" class="'+b.toThemeProperty("jqx-file-upload-button-cancel")+'">'+g+"</button>"),b._bottomButtonsContainer.append(b._cancelButton),b._cancelButton.jqxButton({theme:b.theme,width:h,template:b.cancelTemplate,disabled:b.disabled}),b._bottomButtonsContainer.after('<div style="clear: both;"></div>'),b.rtl&&(b._browseButton.addClass(b.toThemeProperty("jqx-file-upload-button-browse-rtl")),b._cancelButton.addClass(b.toThemeProperty("jqx-file-upload-button-cancel-rtl")),b._uploadButton.addClass(b.toThemeProperty("jqx-file-upload-button-upload-rtl")),a.jqx.browser.msie&&a.jqx.browser.version>8&&b._uploadButton.addClass(b.toThemeProperty("jqx-file-upload-button-upload-rtl-ie"))),b._uploadIframe=a('<iframe name="'+i+'Iframe" class="'+b.toThemeProperty("jqx-file-upload-iframe")+'" src=""></iframe>'),b.host.append(b._uploadIframe),b._iframeInitialized=!1,b._uploadQueue=[],b._forms=[],b._addFormAndFileInput(),b._fileRows=[]},_addFormAndFileInput:function(){var b=this,c=b.element.id,d=a('<form class="'+b.toThemeProperty("jqx-file-upload-form")+'" action="'+b.uploadUrl+'" target="'+c+'Iframe" method="post" enctype="multipart/form-data"></form>');b.host.append(d);var e=a('<input type="file" class="'+b.toThemeProperty("jqx-file-upload-file-input")+'" />');if(b.accept&&e.attr("accept",b.accept),d.append(e),b._ieOldWebkit){var f=b._browseButton.position(),g=b._browseButton.outerWidth(),h=b._browseButton.outerHeight(),i=b.rtl&&b._ie7?12:0;d.css({left:f.left-i,top:f.top,width:g,height:h}),d.addClass(b.toThemeProperty("jqx-file-upload-form-ie9")),e.addClass(b.toThemeProperty("jqx-file-upload-file-input-ie9")),b.addHandler(d,"mouseenter.jqxFileUpload"+c,function(){b._browseButton.addClass(b.toThemeProperty("jqx-fill-state-hover"))}),b.addHandler(d,"mouseleave.jqxFileUpload"+c,function(){b._browseButton.removeClass(b.toThemeProperty("jqx-fill-state-hover"))}),b.addHandler(d,"mousedown.jqxFileUpload"+c,function(){b._browseButton.addClass(b.toThemeProperty("jqx-fill-state-pressed"))}),b.addHandler(a(document),"mouseup.jqxFileUpload"+c,function(){b._browseButton.hasClass("jqx-fill-state-pressed")&&b._browseButton.removeClass(b.toThemeProperty("jqx-fill-state-pressed"))})}b.addHandler(e,"change.jqxFileUpload"+c,function(){var f,g=this.value;a.jqx.browser.mozilla||(g=-1!==g.indexOf("fakepath")?g.slice(12):g.slice(g.lastIndexOf("\\")+1)),f=a.jqx.browser.msie&&a.jqx.browser.version<10?"IE9 and earlier do not support getting the file size.":this.files[0].size;var h=b._addFileRow(g,d,e,f);1===b._fileRows.length&&(b._browseButton.css("margin-bottom","10px"),b._hideButtons(!1)),b._ieOldWebkit&&(b.removeHandler(d,"mouseenter.jqxFileUpload"+c),b.removeHandler(d,"mouseleave.jqxFileUpload"+c),b.removeHandler(d,"mousedown.jqxFileUpload"+c)),b._addFormAndFileInput(),b.removeHandler(e,"change.jqxFileUpload"+c),!0===b.autoUpload&&b._uploadFile(h)}),!0===b._ieOldWebkit&&b.addHandler(e,"click.jqxFileUpload"+c,function(a){!1===b.multipleFilesUpload&&b._fileRows.length>0&&a.preventDefault()}),b._forms.push({form:d,fileInput:e})},_addFileRow:function(b,c,d,e){var f,g,h,i,j,k=this,l="Cancel",m="Upload File";f=a('<div class="'+k.toThemeProperty("jqx-widget-content jqx-rc-all jqx-file-upload-file-row")+'"></div>'),0===k._fileRows.length?k._browseButton.after(f):k._fileRows[k._fileRows.length-1].fileRow.after(f),k.renderFiles?f.html(k.renderFiles(b)):(g=a('<div class="'+k.toThemeProperty("jqx-widget-header jqx-rc-all jqx-file-upload-file-name")+'">'+b+"</div>"),f.append(g),k.localization&&(k.localization.cancelFileTooltip&&(l=k.localization.cancelFileTooltip),k.localization.uploadFileTooltip&&(m=k.localization.uploadFileTooltip)),i=a('<div class="'+k.toThemeProperty("jqx-widget-header jqx-rc-all jqx-file-upload-file-cancel")+'" title="'+l+'"><div class="'+k.toThemeProperty("jqx-icon-close jqx-file-upload-icon")+'"></div></div>'),f.append(i),j=a('<div class="'+k.toThemeProperty("jqx-widget-header jqx-rc-all jqx-file-upload-file-upload")+'" title="'+m+'"><div class="'+k.toThemeProperty("jqx-icon-arrow-up jqx-file-upload-icon jqx-file-upload-icon-upload")+'"></div></div>'),f.append(j),h=a('<div class="'+k.toThemeProperty("jqx-file-upload-loading-element")+'"></div>'),f.append(h),k.rtl&&(g.addClass(k.toThemeProperty("jqx-file-upload-file-name-rtl")),i.addClass(k.toThemeProperty("jqx-file-upload-file-cancel-rtl")),j.addClass(k.toThemeProperty("jqx-file-upload-file-upload-rtl")),h.addClass(k.toThemeProperty("jqx-file-upload-loading-element-rtl"))),k._setMaxWidth({fileNameContainer:g,uploadFile:j,cancelFile:i})),k._ie7&&(k._borderAndPadding("width",f),k._borderAndPadding("height",f),k.renderFiles||(k._borderAndPadding("height",g),k._borderAndPadding("height",j),k._borderAndPadding("height",i)));var n={fileRow:f,fileNameContainer:g,fileName:b,uploadFile:j,cancelFile:i,loadingElement:h,form:c,fileInput:d,index:k._fileRows.length};return k._addFileHandlers(n),k._fileRows.push(n),k._raiseEvent("0",{file:b,size:e}),n},_setMaxWidth:function(a){var b=this,c=a.cancelFile.outerWidth(!0)+a.uploadFile.outerWidth(!0),d=b._ie7?6:0,e=b.host.width()-parseInt(b.host.css("padding-left"),10)-parseInt(b.host.css("padding-right"),10)-c-d-7;a.fileNameContainer.css("max-width",e)},_addFileHandlers:function(a){var b=this;if(!b.renderFiles){var c=b.element.id;b.addHandler(a.uploadFile,"mouseenter.jqxFileUpload"+c,function(){!1===b.disabled&&a.uploadFile.addClass(b.toThemeProperty("jqx-fill-state-hover"))}),b.addHandler(a.uploadFile,"mouseleave.jqxFileUpload"+c,function(){!1===b.disabled&&a.uploadFile.removeClass(b.toThemeProperty("jqx-fill-state-hover"))}),b.addHandler(a.uploadFile,"click.jqxFileUpload"+c,function(){!1===b.disabled&&b._uploadFile(a)}),b.addHandler(a.cancelFile,"mouseenter.jqxFileUpload"+c,function(){!1===b.disabled&&a.cancelFile.addClass(b.toThemeProperty("jqx-fill-state-hover"))}),b.addHandler(a.cancelFile,"mouseleave.jqxFileUpload"+c,function(){!1===b.disabled&&a.cancelFile.removeClass(b.toThemeProperty("jqx-fill-state-hover"))}),b.addHandler(a.cancelFile,"click.jqxFileUpload"+c,function(){!1===b.disabled&&b._removeSingleFileRow(a)})}},_removeSingleFileRow:function(a){var b=this;if(b._removeFileRow(a),b._fileRows.splice(a.index,1),0===b._fileRows.length)setTimeout(function(){b._browseButton.css("margin-bottom",0)},400),b._hideButtons(!0);else for(var c=0;c<b._fileRows.length;c++)b._fileRows[c].index=c},_removeFileRow:function(a){var b=this,c=b.element.id;b.renderFiles||(b.removeHandler(a.uploadFile,"mouseenter.jqxFileUpload"+c),b.removeHandler(a.uploadFile,"mouseleave.jqxFileUpload"+c),b.removeHandler(a.uploadFile,"click.jqxFileUpload"+c),b.removeHandler(a.cancelFile,"mouseenter.jqxFileUpload"+c),b.removeHandler(a.cancelFile,"mouseleave.jqxFileUpload"+c),b.removeHandler(a.cancelFile,"click.jqxFileUpload"+c)),a.fileRow.fadeOut(function(){a.fileRow.remove(),a.form.remove()}),b._raiseEvent("1",{file:a.fileName})},_hideButtons:function(a){var b=this;!0===a?b._bottomButtonsContainer.fadeOut():b._bottomButtonsContainer.fadeIn()},_addHandlers:function(){var b=this,c=b.element.id;b._ieOldWebkit||b.addHandler(b._browseButton,"click.jqxFileUpload"+c,function(){b.browse()}),b.addHandler(b._uploadButton,"click.jqxFileUpload"+c,function(){b.uploadAll()}),b.addHandler(b._cancelButton,"click.jqxFileUpload"+c,function(){b.cancelAll()}),b.addHandler(b._uploadIframe,"load.jqxFileUpload"+c,function(){if((a.jqx.browser.chrome||a.jqx.browser.webkit)&&(b._iframeInitialized=!0),!1===b._iframeInitialized)b._iframeInitialized=!0;else{var c=b._uploadIframe.contents().find("body").html();b._uploadQueue.length>0&&b._raiseEvent("3",{file:b._uploadQueue[b._uploadQueue.length-1].fileName,response:c}),b._fileObjectToRemove&&(b._removeSingleFileRow(b._fileObjectToRemove),b._fileObjectToRemove=null),b._uploadQueue.pop(),b._uploadQueue.length>0&&b._uploadFile(b._uploadQueue[b._uploadQueue.length-1])}})},_removeHandlers:function(b){var c=this,d=c.element.id;if(c.removeHandler(c._browseButton,"click.jqxFileUpload"+d),c.removeHandler(c._uploadButton,"click.jqxFileUpload"+d),c.removeHandler(c._cancelButton,"click.jqxFileUpload"+d),c.removeHandler(c._uploadIframe,"load.jqxFileUpload"+d),!0===b){var e=c._forms[c._forms.length-1];c.removeHandler(e.fileInput,"change.jqxFileUpload"+d),c._ieOldWebkit&&(c.removeHandler(e.form,"mouseenter.jqxFileUpload"+d),c.removeHandler(e.form,"mouseleave.jqxFileUpload"+d),c.removeHandler(e.form,"mousedown.jqxFileUpload"+d),c.removeHandler(a("body"),"mouseup.jqxFileUpload"+d))}}})}(jqxBaseFramework);

