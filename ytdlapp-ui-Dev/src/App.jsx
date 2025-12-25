import { useState } from "react";

function App() {
  const [url, setUrl] = useState("");

  const download = () => {
    if(!url) return alert("لینک وارد نشده");

    // ارسال لینک به بک اند
    window.chrome.webview.postMessage(url);
  };

  return (
    <div style={{padding:30}}>
      <h3>دانلودر یوتیوب</h3>

      <input
        style={{width:"300px",padding:"8px"}}
        placeholder="لینک ویدیو..."
        value={url}
        onChange={e=>setUrl(e.target.value)}
      />

      <button style={{marginLeft:10,padding:"8px 15px"}} onClick={download}>
        دانلود
      </button>
    </div>
  );
}

export default App;