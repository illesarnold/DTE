 private [Type] _[PrivateName]; [Annotations]
    public [Type] [PublicName] { get =&gt; _[PrivateName]; set =&gt; this.RaiseAndSetIfChanged(ref  _[PrivateName], value); } [Comment]
  