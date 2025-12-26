import { useState, useEffect } from "react";

function App() {
  const [url, setUrl] = useState("");
  const [progress, setProgress] = useState(0);

  // دکمه دانلود
  const download = () => {
    if (!url) return alert("لینک وارد نشده!");
    window.chrome.webview.postMessage(url);
  };

  // دریافت پیام از C# (Progress)
  useEffect(() => {
    if (!window.chrome?.webview) return;

    const handler = (e) => {
      const data = e.data;
      if (data.type === "progress") setProgress(data.value);
    };

    window.chrome.webview.addEventListener("message", handler);

    return () => window.chrome.webview.removeEventListener("message", handler);
  }, []);

  return (
    <div style={{ padding: 30 }}>
      <h3>دانلودر یوتیوب</h3>
      <input
        placeholder="لینک ویدیو"
        value={url}
        onChange={e => setUrl(e.target.value)}
        style={{width:300,padding:8}}
      />
      <button onClick={download} style={{marginLeft:10,padding:8}}>دانلود</button>

      <div style={{marginTop:20}}>
        <div>درصد دانلود: {progress}%</div>
        <div style={{width:300,height:20,border:'1px solid #000'}}>
          <div style={{
            width:`${progress}%`,
            height:'100%',
            background:'green'
          }} />
        </div>
      </div>
    </div>
  );
}

export default App;