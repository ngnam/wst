/**
 * @license Copyright (c) 2003-2016, CKSource - Frederico Knabben. All rights reserved.
 * For licensing, see LICENSE.md or http://ckeditor.com/license
 */

CKEDITOR.editorConfig = function( config ) {
	// Define changes to default configuration here. For example:
	 config.language = 'vi';
    // config.uiColor = '#AADC6E';
	 config.height = '500px';
	 config.toolbar = 'Full';

	 config.toolbar_Full =
     [         
         { name: 'clipboard', items: ['Cut', 'Copy', 'Paste', 'PasteText', 'PasteFromWord', '-', 'Undo', 'Redo'] },
         { name: 'editing', items: ['Find', 'Replace', '-', 'SelectAll', '-', 'SpellChecker', 'Scayt'] },
         {
             name: 'forms', items: ['Form', 'Checkbox', 'Radio', 'TextField', 'Textarea', 'Select', 'Button', 'ImageButton',
               'HiddenField']
         },
         '/',
         { name: 'basicstyles', items: ['Bold', 'Italic', 'Underline', 'Strike', 'Subscript', 'Superscript', '-', 'RemoveFormat'] },
         {
             name: 'paragraph', items: ['NumberedList', 'BulletedList', '-', 'Outdent', 'Indent', '-', 'Blockquote', 'CreateDiv',
             '-', 'JustifyLeft', 'JustifyCenter', 'JustifyRight', 'JustifyBlock', '-', 'BidiLtr', 'BidiRtl']
         },
         { name: 'links', items: ['Link', 'Unlink', 'Anchor'] },
         { name: 'insert', items: ['HorizontalRule', 'Smiley', 'SpecialChar'] },
         '/',
         { name: 'styles', items: ['Styles', 'Format', 'Font', 'FontSize'] },
         { name: 'colors', items: ['TextColor', 'BGColor'] },
         { name: 'tools', items: ['Maximize', 'ShowBlocks', '-', 'About'] }
     ];
};
