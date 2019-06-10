if (!String.prototype.endsWith) {
  Object.defineProperty(String.prototype, 'endsWith', {
    value: function (searchString, position) {
      var subjectString = this.toString();
      if (position === undefined || position > subjectString.length) {
        position = subjectString.length;
      }
      position -= searchString.length;
      var lastIndex = subjectString.indexOf(searchString, position);
      return lastIndex !== -1 && lastIndex === position;
    }
  });
}
$.inputformat = {
	defaultOptions: {
		thousandSep: ',',
		decimalSep: '.',
		allowDecimals: true,
		allowNegative: true,
		allowLeadingZero: false,
		maxDecimalDigits: 'unlimited'
        },
	getOptions: function(field){
		return JSON.parse($(field).attr('input-format'));
	},
	getThousandSep: function(field){
		return this.getOptions(field).thousandSep;
	},
	getDecimalSep: function(field){
		return this.getOptions(field).decimalSep;
	},
	getMaxDecimalDigits: function(field){
		return this.getOptions(field).maxDecimalDigits;
	},
	isAllowDecimals: function(field){
		return this.getOptions(field).allowDecimals;
	},
	isAllowNegative: function(field){
		return this.getOptions(field).allowNegative;
	},
	isAllowLeadingZero: function(field){
		return this.getOptions(field).allowLeadingZero;
	},
	getSelectionText: function() {
		    var text = "";
			    if (window.getSelection) {
			        text = window.getSelection().toString();
			    } else if (document.selection && document.selection.type != "Control") {
			        text = document.selection.createRange().text;
		    }
		    return text;
	},
	setCaretPosition : function (ctrl, pos){

		if(ctrl.setSelectionRange){
			ctrl.focus();
			ctrl.setSelectionRange(pos,pos);
		} else if (ctrl.createTextRange) {
			var range = ctrl.createTextRange();
			range.collapse(true);
			range.moveEnd('character', pos);
			range.moveStart('character', pos);
			range.select();
		}
	},
	getCaretPosition: function (oField) {

			  // Initialize
			  var iCaretPos = 0;

			  // IE Support
			  if (document.selection) {

			    // Set focus on the element
			    oField.focus ();

			    // To get cursor position, get empty selection range
			    var oSel = document.selection.createRange ();

			    // Move selection start to 0 position
			    oSel.moveStart ('character', -oField.value.length);

			    // The caret position is selection length
			    iCaretPos = oSel.text.length;
			  }

			  // Firefox support
			  else if (oField.selectionStart || oField.selectionStart == '0')
			    iCaretPos = oField.selectionStart;

			  // Return results
			  return (iCaretPos);
	},
	replaceAll: function(find, replace, str) {
		  return str.replace(new RegExp(find, 'g'), replace);
	},
        unformat: function(field, original){
	     // find part that not decimals
             var rounded = '';
             var decimals = '';
	     var result = '';
	     var _thousandSep = $.inputformat.getThousandSep(field);	
	     var _decimalSep = $.inputformat.getDecimalSep(field);
   	     var _isAllowLeadingZero = $.inputformat.isAllowLeadingZero(field);	

	     if(original=='0') return '0';	
	     // find decimal is exists
             var dot = original.indexOf(_decimalSep);
             if(dot == -1){
                   // no decimal
		   rounded = original;
             }else {
                   // have decimal
		   rounded = original.substring(0, dot);
                   if(dot < original.length-1){
                      decimals = original.substring(dot+1);
                   } else decimals = '';
             }
	     // Remove all thousand separators from rounded
             rounded = $.inputformat.replaceAll(_thousandSep, '', rounded.toString());
             if(decimals!==''){
		  result = rounded + _decimalSep + decimals; 
	     }
	     else result = rounded;

	     if(!_isAllowLeadingZero){
		// remove all leading zero when unformatting
		// result = "" + parseFloat(result);		
		while(result.charAt(0)=='0'){
			result = result.substring(1);
		}
	     }
             return result;
        },
        format: function(field, original){
             var unformatted = (original=='0') ? 0 : $.inputformat.unformat(field, original);
	     var rounded = '';
             var decimals = '';
	     var _thousandSep = $.inputformat.getThousandSep(field);	
	     var _decimalSep = $.inputformat.getDecimalSep(field);
	
	
             // find decimal is exists
             var dot = unformatted.indexOf(_decimalSep);
             if(dot == -1){
                   // no decimal
		   rounded = unformatted;
             }else {
                   // have decimal
		   rounded = unformatted.substring(0, dot);
                   if(dot <= unformatted.length-1){
                      decimals = unformatted.substring(dot+1);
                   } else decimals = '';
             }
	     var formatted = rounded.toString().replace(/(\d)(?=(\d{3})+(?!\d))/g, "$1" + _thousandSep);
	     if(dot != -1){
               // if dot at the end, do not remove it
	       if(decimals!=='') 
                   formatted = formatted + _decimalSep + decimals; 
	     }
             return formatted;
        }
};
$.fn.inputNumber = function(options){
   	
   if(options!==undefined){
	
	if(options.thousandSep===undefined) options.thousandSep = $.inputformat.defaultOptions.thousandSep;
	if(options.decimalSep===undefined) options.decimalSep = $.inputformat.defaultOptions.decimalSep;
	if(options.maxDecimalDigits!==undefined) {
		if(options.maxDecimalDigits!='unlimited' && isNaN(options.maxDecimalDigits)){
			throw "maxDecimalDigits must a number or 'unlimited'";
		} 
	} else options.maxDecimalDigits = $.inputformat.defaultOptions.maxDecimalDigits;
	if(options.allowDecimals!==undefined) {
		if(options.allowDecimals!==true && options.allowDecimals!==false){
			throw "allowDecimals must boolean";
		}
	} else options.allowDecimals = $.inputformat.defaultOptions.allowDecimals;
	if(options.allowNegative!==undefined) {
		if(options.allowNegative!==true && options.allowNegative!==false){
			throw "allowNegative must boolean";
		}
	} else options.allowNegative = $.inputformat.defaultOptions.allowNegative;	
	if(options.allowLeadingZero!==undefined) {
		if(options.allowLeadingZero!==true && options.allowLeadingZero!==false){
			throw "allowNegative must boolean";
		}
	} else options.allowLeadingZero = $.inputformat.defaultOptions.allowLeadingZero;

	if(options.numericOnly!==undefined){
		if(options.numericOnly!==true && options.numericOnly!==false){
			throw "numericOnly must boolean";
		}
		if(options.numericOnly===true){
			// thousandSep = ''
			// allowDecimals = false
			// allowNegative = false
			options.allowDecimals = false;
			options.allowNegative = false;
			options.thousandSep = '';
		}
	}
   } else options = $.inputformat.defaultOptions;

   $(this).attr('input-format', JSON.stringify(options));			

   $(this).on('paste', function(){
	var that = this;
	setTimeout(function () {
		var unformatted = $.inputformat.unformat(that, that.value); 
		if(isNaN(unformatted)) {
			$(that).val($.inputformat.oldValue);
		} else {
		      // Validate rules
		      // Check with allow negative
		      // Check with allow decimals
		      // Check with allow leading zero
                      // Check with max decimal digits	 
		      $(that).val( $.inputformat.format(that, unformatted) );
		}
	},0);
   });
   $(this).keyup(function(event){
	 var pos = $.inputformat.getCaretPosition(this);
         var that = this;
         var value = $(that).val();

	 if(event.ctrlKey){
	      switch(event.keyCode){
		case 65: // Ctrl A
                case 67: // Ctrl C
		case 88: // Ctrl X
		case 86: // Ctrl V
                   return;
	      }
         }
  	 switch(event.keyCode){
		case 16: // SHIFT UP
		case 17: // CTRL UP
		case 36: // HOME
		case 35: // END
		case 37: // LEFT ARROW
		case 39: // RIGHT ARROW			
                    break;

		case 96: // from numpad 0    

		// Number from Numlock 1-9
		case 97: case 98: case 99: case 100: case 101: case 102:
		case 103: case 104: case 105:

		// Only do formatting if input numbers, BKSPACE, DELETE
 		case 48: case 49: case 50: case 51: case 52: case 53: case 54:
                case 55: case 56: case 57:
		case 8:  // BACK SPACE
		case 46: //DELETE
		case 189: // '-'
			 setTimeout( function(){
		             var formatted = $.inputformat.format(that, value);
			     var diff = formatted.length - value.length;
			     $(that).val( formatted );
			     $.inputformat.setCaretPosition(that, pos + diff); 
			 }, 0); 
	 }
   });
   $(this).keydown(function(event){

         var pos = $.inputformat.getCaretPosition(this);
	 	 var value = $(this).val();
	 	 var _thousandSep = $.inputformat.getThousandSep(this);
	 	 var _decimalSep = $.inputformat.getDecimalSep(this);
	 	 var _maxDigits = $.inputformat.getMaxDecimalDigits(this);
	 	 var _isAllowLeadingZero = $.inputformat.isAllowLeadingZero(this);

	 $.inputformat.oldValue = value;
	 if(event.ctrlKey){
	      switch(event.keyCode){
				case 65: // Ctrl A
		                case 67: // Ctrl C
				        break;	
				case 88: // Ctrl X
					event.preventDefault();
					break;
				case 86: // Ctrl V
					event.preventDefault();
					break;			
				default:
					event.preventDefault();
              }	
              return;
         }	

         switch(event.keyCode){
	        // must numeric
	    case 96: // from numpad 0    
		case 48: 
			// Check allow leading zero
			if(!_isAllowLeadingZero){
				if($.inputformat.getSelectionText()==value){}
				// leading zero is not allowed
				else if(pos===0 && value!=='') {
					event.preventDefault();
				}	
				else if(pos==1 && value==='0') {
					event.preventDefault();
				}
			}

			// If type number behind ., limit by maxDecimalDigits
			if(_maxDigits=='unlimited') break; // by pass if unlimited
			else {
			    // Check whether behind decimalSep already have number equals to max
			    // Cursor pos must after decimal sep to be checken	
			    var n = value.lastIndexOf(_decimalSep);
			    if(n!=-1 && pos > n && value.length - n == _maxDigits+1) {
				event.preventDefault();			
			    }	
			}
			break;

		// Number from Numlock 1-9
		case 97: case 98: case 99: case 100: case 101: case 102:
		case 103: case 104: case 105:

		// Number	
		case 49: case 50: case 51: case 52: case 53: case 54:
                case 55: case 56: case 57:
                  if(pos===0 && value.indexOf('-')===0) event.preventDefault(); 

		  // Check allow leading zero
		  if(!_isAllowLeadingZero){
				// leading zero is not allowed
				if(pos==1 && value==='0') event.preventDefault();
		  }	

		  // Check to max decimal digits
		  // Cursor pos must after decimal sep to be checken	

			if(_maxDigits=='unlimited') break; // by pass if unlimited
			else {
			    // Check whether behind decimalSep already have number equals to max
			    // Cursor pos must after decimal sep to be checken	
			    var m = value.lastIndexOf(_decimalSep);
			    if(m!=-1 && pos > m && value.length - m == _maxDigits+1) {
				event.preventDefault();			
			    }	
			}

                  break;
		case 36: // HOME
		case 9:  // TAB
		case 8:  // BACK SPACE
			if(value.substr(pos-1,1)==_thousandSep){
				// remove the thousandSep immediately
                                value = value.substr(0,pos) + value.substr(pos);
				$(this).val(value);
                        }
			break;
		case 46: //DELETE
		case 35: // END
		case 37: // LEFT ARROW
		case 39: // RIGHT ARROW
		  break;

		case 110: // '.' from num pad  
		case 190: // '.'
	
		  // If allowDecimals set to false, prevent it immediately
		  if($.inputformat.isAllowDecimals(this)!==true) event.preventDefault();

                  // only once, check existing must have no .
		  if(value.indexOf(_decimalSep)!=-1) event.preventDefault();
		  // must only at end
	          if(pos < value.length) event.preventDefault(); 		
                  break;
        case 109: // '-' from numpad          
		case 189: // '-'
		  // If allowNegative set to false, prevent it immediately
		  if($.inputformat.isAllowNegative(this)!==true) event.preventDefault();

                  // only can on beginning of value
                  if(pos>0){
			event.preventDefault();
		  }else{
			// check current value cannot start with -
                        if(value.indexOf('-')===0) event.preventDefault();
		  }
		  break;
                default:
                  event.preventDefault();
         }
   });
};
