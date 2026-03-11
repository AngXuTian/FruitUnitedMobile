import './index.css';

function App() {
    return (
        <div className="p-6 bg-green-500 text-white text-xl rounded-lg shadow-lg">
            Tailwind is now working ✅
        </div>
    );
}

export default App;

import { createRoot } from 'react-dom/client';
const container = document.getElementById('react-root');
if (container) createRoot(container).render(<App />);
