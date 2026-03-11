<%@ Control Language="C#" AutoEventWireup="true" %>

<style>
    .loader-overlay {
        position: fixed;
        top: 0; left: 0;
        width: 100vw; height: 100vh;
        background-color: rgba(247, 249, 252, 0.98);
        z-index: 9999;
        display: none; 
        flex-direction: column;
        align-items: center;
        justify-content: center;
        transition: opacity 0.5s ease;
        opacity: 0;
    }

    .truck-scene {
        position: relative;
        width: 90%; /* Responsive width */
        max-width: 500px; /* Limit size on tablets/desktops */
        height: 250px;
        background: linear-gradient(to bottom, #87CEEB 0%, #fff 100%);
        border-bottom: 4px solid #777;
        overflow: hidden;
        border-radius: 15px;
    }

    .truck-wrap {
        position: absolute;
        bottom: 10px;
        left: -150px; /* Start off-screen */
        width: 140px; /* Scaled down for mobile */
        height: 80px;
        transition: transform 2s cubic-bezier(0.45, 0.05, 0.55, 0.95);
    }

    /* Use flexbox to keep truck parts together */
    .truck-body {
        position: absolute;
        bottom: 20px;
        width: 80px; height: 45px;
        background: #5d4037;
        border-radius: 3px;
        display: flex; flex-wrap: wrap;
        align-content: flex-end; padding: 3px;
        box-sizing: border-box;
    }

    .truck-cabin {
        position: absolute;
        bottom: 20px; right: 10px;
        width: 45px; height: 40px;
        background: #e53935;
        border-radius: 4px 15px 4px 2px;
    }

    .wheel {
        position: absolute; bottom: 0;
        width: 20px; height: 20px;
        background: #222; border-radius: 50%;
        border: 3px dashed #555;
        animation: rotate-wheel 0.5s infinite linear;
    }
    .w-back { left: 15px; }
    .w-front { right: 25px; }

    .fruit-item {
        font-size: 1.2rem; position: absolute;
        opacity: 0;
        animation: drop-fruit 0.4s ease-out forwards;
    }

    @keyframes rotate-wheel { 100% { transform: rotate(360deg); } }
    @keyframes drop-fruit {
        0% { transform: translateY(-100px); opacity: 0; }
        100% { transform: translateY(0); opacity: 1; }
    }

    .loading-status {
        margin-top: 20px;
        font-family: sans-serif;
        color: #2e7d32;
        font-size: 14px;
        text-align: center;
        padding: 0 20px;
    }
</style>

<div id="FruitLoaderPanel" class="loader-overlay">
    <div class="truck-scene" id="SceneBoundary">
        <div style="position:absolute; right:10px; bottom:10px; font-size:3.5rem;">🏪</div>
        <div id="TheTruck" class="truck-wrap">
            <div id="TruckBed" class="truck-body"></div>
            <div class="truck-cabin"></div>
            <div class="wheel w-back"></div>
            <div class="wheel w-front"></div>
        </div>
    </div>
    <div id="StatusText" class="loading-status">Packing your fruit order...</div>
</div>

<script type="text/javascript">
    let fruitInterval;
    let loadingStartTime;

    function showFruitLoader() {
        const fruits = ['🍎', '🍌', '🍇', '🍊', '🍋', '🍐', '🍓', '🍍'];
        const overlay = document.getElementById('FruitLoaderPanel');
        const truck = document.getElementById('TheTruck');
        const bed = document.getElementById('TruckBed');

        loadingStartTime = Date.now();
        bed.innerHTML = '';
        overlay.style.display = 'flex';
        setTimeout(() => { overlay.style.opacity = '1'; }, 10);

        // Calculate mid-point based on container size
        const midPoint = (document.getElementById('SceneBoundary').offsetWidth / 2) + 50;
        truck.style.transform = `translateX(${midPoint}px)`;

        fruitInterval = setInterval(() => {
            const f = document.createElement('span');
            f.className = 'fruit-item';
            f.innerText = fruits[Math.floor(Math.random() * fruits.length)];
            f.style.left = (Math.random() * 60) + "px";
            bed.appendChild(f);
        }, 300);
    }

    function hideFruitLoader() {
        const overlay = document.getElementById('FruitLoaderPanel');
        const truck = document.getElementById('TheTruck');
        const status = document.getElementById('StatusText');

        const elapsed = Date.now() - loadingStartTime;
        const minWait = 1800;
        const extraWait = Math.max(0, minWait - elapsed);

        setTimeout(() => {
            clearInterval(fruitInterval);
            status.innerText = "Order ready! Delivering...";

            // Drive to shop (calculate end point dynamically)
            const endPoint = document.getElementById('SceneBoundary').offsetWidth + 100;
            truck.style.transform = `translateX(${endPoint}px)`;

            setTimeout(() => {
                overlay.style.opacity = '0';
                setTimeout(() => { overlay.style.display = 'none'; }, 500);
            }, 2000);
        }, extraWait);
    }
</script>