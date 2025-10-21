// videoInterop.js - Video player JavaScript interop module
export function seekTo(videoElement, timeInSeconds) {
    if (!videoElement || typeof timeInSeconds !== 'number') {
        console.error('Invalid parameters for seekTo');
        return;
    }

    try {
        videoElement.currentTime = timeInSeconds;
    } catch (error) {
        console.error('Error seeking video:', error);
    }
}

export function play(videoElement) {
    if (!videoElement) {
        console.error('Invalid video element');
        return;
    }

    try {
        return videoElement.play();
    } catch (error) {
        console.error('Error playing video:', error);
    }
}

export function pause(videoElement) {
    if (!videoElement) {
        console.error('Invalid video element');
        return;
    }

    try {
        videoElement.pause();
    } catch (error) {
        console.error('Error pausing video:', error);
    }
}

export function getCurrentTime(videoElement) {
    if (!videoElement) {
        console.error('Invalid video element');
        return 0;
    }

    try {
        return videoElement.currentTime || 0;
    } catch (error) {
        console.error('Error getting current time:', error);
        return 0;
    }
}

export function getDuration(videoElement) {
    if (!videoElement) {
        console.error('Invalid video element');
        return 0;
    }

    try {
        return videoElement.duration || 0;
    } catch (error) {
        console.error('Error getting duration:', error);
        return 0;
    }
}

export function setVolume(videoElement, volume) {
    if (!videoElement || typeof volume !== 'number') {
        console.error('Invalid parameters for setVolume');
        return;
    }

    try {
        // Volume must be between 0 and 1
        videoElement.volume = Math.max(0, Math.min(1, volume));
    } catch (error) {
        console.error('Error setting volume:', error);
    }
}

export function getVolume(videoElement) {
    if (!videoElement) {
        console.error('Invalid video element');
        return 1;
    }

    try {
        return videoElement.volume || 1;
    } catch (error) {
        console.error('Error getting volume:', error);
        return 1;
    }
}

export function setPlaybackRate(videoElement, rate) {
    if (!videoElement || typeof rate !== 'number') {
        console.error('Invalid parameters for setPlaybackRate');
        return;
    }

    try {
        // Playback rate typically between 0.25 and 2.0
        videoElement.playbackRate = Math.max(0.25, Math.min(2.0, rate));
    } catch (error) {
        console.error('Error setting playback rate:', error);
    }
}

export function getPlaybackRate(videoElement) {
    if (!videoElement) {
        console.error('Invalid video element');
        return 1;
    }

    try {
        return videoElement.playbackRate || 1;
    } catch (error) {
        console.error('Error getting playback rate:', error);
        return 1;
    }
}

export function isPlaying(videoElement) {
    if (!videoElement) {
        console.error('Invalid video element');
        return false;
    }

    try {
        return !videoElement.paused && !videoElement.ended && videoElement.readyState > 2;
    } catch (error) {
        console.error('Error checking if video is playing:', error);
        return false;
    }
}

export function toggleFullscreen(videoElement) {
    if (!videoElement) {
        console.error('Invalid video element');
        return;
    }

    try {
        if (!document.fullscreenElement) {
            if (videoElement.requestFullscreen) {
                videoElement.requestFullscreen();
            } else if (videoElement.webkitRequestFullscreen) {
                videoElement.webkitRequestFullscreen();
            } else if (videoElement.msRequestFullscreen) {
                videoElement.msRequestFullscreen();
            }
        } else {
            if (document.exitFullscreen) {
                document.exitFullscreen();
            } else if (document.webkitExitFullscreen) {
                document.webkitExitFullscreen();
            } else if (document.msExitFullscreen) {
                document.msExitFullscreen();
            }
        }
    } catch (error) {
        console.error('Error toggling fullscreen:', error);
    }
}
