<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Toast.ascx.cs" Inherits="FruitUnitedMobile.Component.Toast" %>

<div class="toast-container position-fixed p-3">
    <div id="commonToast" class="toast" role="alert" aria-live="assertive" aria-atomic="true">
        <div class="toast-header">
            <i class="toast-icon me-2"></i>
            <strong class="me-auto toast-title">Notification</strong>
            <small class="toast-time">just now</small>
            <button type="button" class="btn-close" data-bs-dismiss="toast" aria-label="Close"></button>
        </div>
        <div class="toast-body">
            Toast message here
        </div>
    </div>
</div>

<style>
    /* Toast Container - centered top, below header (150px default) */
    .toast-container {
        top: 120px;
        left: 50%;
        transform: translateX(-50%);
        z-index: 9999;
        width: auto;
        max-width: 100%;
        pointer-events: none;
        position: fixed;
        padding: 1rem;
    }

    .toast-container > * {
        pointer-events: auto;
    }

    /* Toast Styles */
    .toast {
        min-width: 320px;
        max-width: 500px;
        box-shadow: 0 6px 16px rgba(0, 0, 0, 0.12), 0 3px 6px rgba(0, 0, 0, 0.08);
        border-radius: 10px;
        border: none;
        overflow: hidden;
        animation: slideInBounce 0.5s cubic-bezier(0.68, -0.55, 0.265, 1.55);
        transition: all 0.3s ease;
        background-color: white;
        position: relative;
    }

    @keyframes slideInBounce {
        0% {
            transform: translateY(-120%);
            opacity: 0;
        }
        60% {
            transform: translateY(10px);
            opacity: 1;
        }
        80% {
            transform: translateY(-5px);
        }
        100% {
            transform: translateY(0);
            opacity: 1;
        }
    }

    @keyframes slideOutUp {
        0% {
            transform: translateY(0);
            opacity: 1;
        }
        100% {
            transform: translateY(-120%);
            opacity: 0;
        }
    }

    .toast.hiding {
        animation: slideOutUp 0.3s ease-out forwards;
    }

    /* Toast Header */
    .toast-header {
        padding: 12px 14px;
        font-weight: 600;
        border-radius: 10px 10px 0 0;
        font-size: 0.95rem;
    }

    /* Toast Body */
    .toast-body {
        padding: 14px 16px;
        font-size: 0.9rem;
        word-wrap: break-word;
        line-height: 1.5;
    }

    /* Toast Icon */
    .toast-icon::before {
        display: inline-flex;
        align-items: center;
        justify-content: center;
        width: 22px;
        height: 22px;
        border-radius: 50%;
        text-align: center;
        line-height: 22px;
        font-weight: bold;
        box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
    }

    /* Toast Types */
    .toast.toast-success .toast-header {
        background: linear-gradient(135deg, #e8f5e9 0%, #c8e6c9 100%);
        color: #2e7d32;
        border-left: 4px solid #4caf50;
    }
    .toast.toast-success .toast-icon::before {
        content: "✓";
        background: linear-gradient(135deg, #66bb6a 0%, #4caf50 100%);
        color: white;
        font-size: 13px;
    }
    .toast.toast-success .toast-body {
        border-left: 4px solid #4caf50;
    }

    .toast.toast-error .toast-header {
        background: linear-gradient(135deg, #ffebee 0%, #ffcdd2 100%);
        color: #c62828;
        border-left: 4px solid #f44336;
    }
    .toast.toast-error .toast-icon::before {
        content: "✕";
        background: linear-gradient(135deg, #ef5350 0%, #f44336 100%);
        color: white;
        font-size: 14px;
    }
    .toast.toast-error .toast-body {
        border-left: 4px solid #f44336;
    }

    .toast.toast-warning .toast-header {
        background: linear-gradient(135deg, #fff8e1 0%, #ffecb3 100%);
        color: #f57c00;
        border-left: 4px solid #ff9800;
    }
    .toast.toast-warning .toast-icon::before {
        content: "⚠";
        background: linear-gradient(135deg, #ffa726 0%, #ff9800 100%);
        color: white;
        font-size: 14px;
        justify-content: center;
    }
    .toast.toast-warning .toast-body {
        border-left: 4px solid #ff9800;
    }

    .toast.toast-info .toast-header {
        background: linear-gradient(135deg, #e3f2fd 0%, #bbdefb 100%);
        color: #1565c0;
        border-left: 4px solid #2196f3;
    }
    .toast.toast-info .toast-icon::before {
        content: "ℹ";
        background: linear-gradient(135deg, #42a5f5 0%, #2196f3 100%);
        color: white;
        font-size: 15px;
    }
    .toast.toast-info .toast-body {
        border-left: 4px solid #2196f3;
    }

    /* Close Button */
    .toast .btn-close {
        font-size: 0.875rem;
        opacity: 0.6;
        transition: opacity 0.2s ease, transform 0.2s ease;
    }
    .toast .btn-close:hover {
        opacity: 1;
        transform: scale(1.1);
    }

    /* Time badge */
    .toast-time {
        opacity: 0.7;
        font-size: 0.75rem;
    }

    /* Responsive */
    @media (max-width: 576px) {
        .toast-container {
            top: 130px !important; /* Increased top space on small screens */
            left: 50%;
            transform: translateX(-50%);
            padding: 0 !important;
        }

        .toast {
            min-width: 90vw;
            max-width: 90vw;
            border-radius: 8px;
        }

        .toast-header {
            border-radius: 8px 8px 0 0;
        }

        .toast-body {
            border-radius: 0 0 8px 8px;
            font-size: 0.85rem;
        }
    }

    /* Progress bar (optional) */
    .toast::after {
        content: '';
        position: absolute;
        bottom: 0;
        left: 0;
        height: 3px;
        width: 0;
        background: currentColor;
        opacity: 0.3;
        animation: progressBar 5s linear forwards;
    }

    @keyframes progressBar {
        to { width: 100%; }
    }

    .toast.no-progress::after {
        display: none;
    }
</style>

<script type="text/javascript">
    function showToast(message, type, title, duration) {
        type = type || 'info';
        duration = typeof duration !== 'undefined' ? duration : 5000;

        var toastElement = document.getElementById('commonToast');
        if (!toastElement) {
            console.error('Toast element not found!');
            return;
        }

        toastElement.classList.remove('toast-success', 'toast-error', 'toast-warning', 'toast-info', 'hiding', 'no-progress');
        toastElement.classList.add('toast-' + type);

        if (duration === 0) {
            toastElement.classList.add('no-progress');
        }

        var titles = {
            'success': 'Success',
            'error': 'Error',
            'warning': 'Warning',
            'info': 'Information'
        };

        var toastTitle = title || titles[type] || 'Notification';
        toastElement.querySelector('.toast-title').textContent = toastTitle;
        toastElement.querySelector('.toast-body').innerHTML = message;

        var now = new Date();
        var timeString = now.toLocaleTimeString('en-US', {
            hour: 'numeric',
            minute: '2-digit',
            hour12: true
        });
        toastElement.querySelector('.toast-time').textContent = timeString;

        var bsToast = new bootstrap.Toast(toastElement, {
            autohide: duration > 0,
            delay: duration
        });

        if (duration > 0) {
            setTimeout(function () {
                toastElement.classList.add('hiding');
            }, duration - 300);
        }

        bsToast.show();
    }
</script>
