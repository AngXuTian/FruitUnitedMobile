<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FruitLoader.ascx.cs" Inherits="FruitUnitedMobile.Component.FruitLoader" %>

<style>
    .loader-overlay {
        position: fixed;
        top: 0; 
        left: 0;
        width: 100vw; 
        height: 100vh;
        background-color: rgba(247, 249, 252, 0.98);
        z-index: 99999;
        display: none; /* Hidden by default */
        flex-direction: column;
        align-items: center;
        justify-content: center;
        opacity: 0;
        transition: opacity 0.3s ease;
    }

    /* Force the loader to show when this class is added via JS */
    .loader-overlay.is-active {
        display: flex !important;
        opacity: 1 !important;
    }

    .truck-scene {
        position: relative;
        width: 90%;
        max-width: 500px;
        height: 200px;
        background: linear-gradient(90deg, #87CEEB 0%, #fff 50%, #87CEEB 100%);
        background-size: 200% 100%;
        border-bottom: 4px solid #777;
        overflow: hidden;
        border-radius: 15px;
        animation: drive-bg 1.5s linear infinite;
    }

    @keyframes drive-bg {
        from { background-position: 200% 0; }
        to { background-position: 0% 0; }
    }

    .truck-wrap {
        position: absolute;
        bottom: 10px; 
        left: 50%;
        transform: translateX(-50%);
        width: 140px;
        height: 80px;
        animation: truck-jiggle 0.2s infinite ease-in-out;
    }

    @keyframes truck-jiggle {
        0%, 100% { transform: translateX(-50%) translateY(0); }
        50% { transform: translateX(-50%) translateY(-3px); }
    }

    .truck-body {
        position: absolute;
        bottom: 20px; 
        width: 80px; height: 45px;
        background: #5d4037;
        border-radius: 3px;
        z-index: 2;
    }

    .truck-cabin {
        position: absolute;
        bottom: 20px; right: 10px;
        width: 45px; height: 40px;
        background: #e53935;
        border-radius: 4px 15px 4px 2px;
        z-index: 2;
    }

    .wheel {
        position: absolute;
        bottom: 0;
        width: 20px; height: 20px;
        background: #222; border-radius: 50%;
        border: 3px dashed #555;
        animation: rotate-wheel 0.5s infinite linear;
        z-index: 3;
    }
    .w-back { left: 15px; }
    .w-front { right: 25px; }

    .fruit-item {
        font-size: 1.5rem;
        position: absolute;
        bottom: 5px; 
        opacity: 0;
        animation: drop-fruit 0.4s ease-out forwards;
        z-index: 1;
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
        font-weight: bold;
    }
</style>

<div id="FruitLoaderPanel" class="loader-overlay">
    <div class="truck-scene">
        <div id="TheTruck" class="truck-wrap">
            <div id="TruckBed" class="truck-body"></div>
            <div class="truck-cabin"></div>
            <div class="wheel w-back"></div>
            <div class="wheel w-front"></div>
        </div>
    </div>
    <div id="StatusText" class="loading-status">Delivering your fruit order...</div>
</div>

<script type="text/javascript">
    var fruitInterval;
    var loadingStartTime;

    function showFruitLoader() {
        var overlay = document.getElementById('FruitLoaderPanel');
        var bed = document.getElementById('TruckBed');
        var fruits = ['🍎', '🍌', '🍇', '🍊', '🍋', '🍐', '🍓', '🍍'];

        if (!overlay) return;

        loadingStartTime = Date.now();
        bed.innerHTML = '';

        // Use the class instead of direct style manipulation [cite: 29, 30]
        overlay.classList.add('is-active');

        if (fruitInterval) clearInterval(fruitInterval);

        fruitInterval = setInterval(function () {
            var f = document.createElement('span');
            f.className = 'fruit-item';
            f.innerText = fruits[Math.floor(Math.random() * fruits.length)];
            f.style.left = (Math.random() * 55) + "px";
            bed.appendChild(f);

            if (bed.children.length > 10) {
                bed.removeChild(bed.firstChild);
            }
        }, 400);
    }

    function hideFruitLoader() {
        var overlay = document.getElementById('FruitLoaderPanel');
        if (!overlay) return;

        var elapsed = Date.now() - loadingStartTime;
        var minWait = 1000;
        var extraWait = Math.max(0, minWait - elapsed);

        setTimeout(function () {
            clearInterval(fruitInterval);
            overlay.classList.remove('is-active');
        }, extraWait);
    }
</script>