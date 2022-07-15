mergeInto(LibraryManager.library, {

  HelloString: function (str) {
    ReactUnityWebGL.HelloString(UTF8ToString(str));
  }

});