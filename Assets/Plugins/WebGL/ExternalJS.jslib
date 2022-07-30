mergeInto(LibraryManager.library, {

  HelloString: function (str) {
    ReactUnityWebGL.HelloString(UTF8ToString(str));
  },

  HelloString2: function (str2) {
    ReactUnityWebGL.HelloString2(str2);
  }

});