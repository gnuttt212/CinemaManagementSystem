/* ============================================================
   CINEMA GSAP ANIMATION ENGINE
   Premium scroll-driven & entrance animations
   ============================================================ */

document.addEventListener('DOMContentLoaded', () => {
    // Register GSAP plugins
    gsap.registerPlugin(ScrollTrigger);

    // ─── NAVBAR SCROLL EFFECT ───
    initNavbar();

    // ─── HERO ANIMATIONS ───
    initHero();

    // ─── SCROLL REVEAL ANIMATIONS ───
    initScrollReveals();

    // ─── MOVIE CARD STAGGER ───
    initMovieCards();

    // ─── FEATURE CARDS ───
    initFeatureCards();

    // ─── PARALLAX FLOATING ORBS ───
    initFloatingOrbs();

    // ─── SCROLL INDICATOR ───
    initScrollIndicator();

    // ─── MAGNETIC BUTTONS ───
    initMagneticButtons();

    // ─── COUNTER ANIMATIONS ───
    initCounters();

    // ─── PAGE TRANSITION ───
    initPageTransition();
});

/* ═══════════════════════════════════════════
   NAVBAR - Shrink on scroll + reveal
   ═══════════════════════════════════════════ */
function initNavbar() {
    const navbar = document.querySelector('.cinema-navbar');
    if (!navbar) return;

    // Initial reveal
    gsap.from(navbar, {
        y: -100,
        opacity: 0,
        duration: 1,
        ease: 'power3.out',
        delay: 0.2
    });

    // Nav items stagger
    gsap.from('.cinema-navbar .nav-item', {
        y: -20,
        opacity: 0,
        duration: 0.6,
        stagger: 0.1,
        ease: 'power2.out',
        delay: 0.6
    });

    // Scroll shrink
    ScrollTrigger.create({
        start: 'top -80',
        onUpdate: (self) => {
            if (self.direction === 1) {
                navbar.classList.add('scrolled');
            } else if (window.scrollY < 80) {
                navbar.classList.remove('scrolled');
            }
        }
    });
}

/* ═══════════════════════════════════════════
   HERO - Cinematic text reveal + parallax
   ═══════════════════════════════════════════ */
function initHero() {
    const hero = document.querySelector('.gsap-hero');
    if (!hero) return;

    const tl = gsap.timeline({ defaults: { ease: 'power4.out' } });

    // Background zoom
    tl.from('.gsap-hero-bg', {
        scale: 1.3,
        duration: 2,
        ease: 'power2.out'
    }, 0);

    // Title lines reveal
    tl.to('.gsap-hero-title .line-inner', {
        y: 0,
        duration: 1.2,
        stagger: 0.15,
        ease: 'power4.out'
    }, 0.3);

    // Subtitle fade
    tl.to('.gsap-hero-subtitle', {
        opacity: 1,
        y: 0,
        duration: 0.8
    }, 1);

    // CTA buttons
    tl.to('.gsap-hero-cta', {
        opacity: 1,
        y: 0,
        duration: 0.8
    }, 1.2);

    // Parallax on scroll
    gsap.to('.gsap-hero-bg', {
        yPercent: 30,
        ease: 'none',
        scrollTrigger: {
            trigger: hero,
            start: 'top top',
            end: 'bottom top',
            scrub: true
        }
    });

    // Fade out hero content on scroll
    gsap.to('.gsap-hero-content', {
        opacity: 0,
        y: -80,
        ease: 'none',
        scrollTrigger: {
            trigger: hero,
            start: '30% top',
            end: '80% top',
            scrub: true
        }
    });
}

/* ═══════════════════════════════════════════
   SCROLL REVEALS - Generic elements
   ═══════════════════════════════════════════ */
function initScrollReveals() {
    // Fade up reveals
    gsap.utils.toArray('.gsap-reveal').forEach(el => {
        gsap.to(el, {
            opacity: 1,
            y: 0,
            duration: 0.8,
            ease: 'power3.out',
            scrollTrigger: {
                trigger: el,
                start: 'top 85%',
                toggleActions: 'play none none none'
            }
        });
    });

    // Left reveals
    gsap.utils.toArray('.gsap-reveal-left').forEach(el => {
        gsap.to(el, {
            opacity: 1,
            x: 0,
            duration: 0.8,
            ease: 'power3.out',
            scrollTrigger: {
                trigger: el,
                start: 'top 85%',
                toggleActions: 'play none none none'
            }
        });
    });

    // Right reveals
    gsap.utils.toArray('.gsap-reveal-right').forEach(el => {
        gsap.to(el, {
            opacity: 1,
            x: 0,
            duration: 0.8,
            ease: 'power3.out',
            scrollTrigger: {
                trigger: el,
                start: 'top 85%',
                toggleActions: 'play none none none'
            }
        });
    });

    // Scale reveals
    gsap.utils.toArray('.gsap-reveal-scale').forEach(el => {
        gsap.to(el, {
            opacity: 1,
            scale: 1,
            duration: 0.8,
            ease: 'back.out(1.7)',
            scrollTrigger: {
                trigger: el,
                start: 'top 85%',
                toggleActions: 'play none none none'
            }
        });
    });
}

/* ═══════════════════════════════════════════
   MOVIE CARDS - Stagger reveal
   ═══════════════════════════════════════════ */
function initMovieCards() {
    const cards = gsap.utils.toArray('.gsap-movie-card, .gsap-schedule-card');
    if (!cards.length) return;

    // Group cards by rows for staggering
    const rows = {};
    cards.forEach(card => {
        const rect = card.getBoundingClientRect();
        const rowKey = Math.round(rect.top / 50);
        if (!rows[rowKey]) rows[rowKey] = [];
        rows[rowKey].push(card);
    });

    // Animate each batch
    cards.forEach((card, i) => {
        gsap.from(card, {
            opacity: 0,
            y: 60,
            scale: 0.92,
            duration: 0.7,
            delay: (i % 4) * 0.12,
            ease: 'power3.out',
            scrollTrigger: {
                trigger: card,
                start: 'top 90%',
                toggleActions: 'play none none none'
            }
        });
    });

    // Tilt effect on hover
    cards.forEach(card => {
        card.addEventListener('mousemove', (e) => {
            const rect = card.getBoundingClientRect();
            const x = e.clientX - rect.left;
            const y = e.clientY - rect.top;
            const centerX = rect.width / 2;
            const centerY = rect.height / 2;
            const rotateX = (y - centerY) / 20;
            const rotateY = (centerX - x) / 20;

            gsap.to(card, {
                rotateX: rotateX,
                rotateY: rotateY,
                duration: 0.4,
                ease: 'power2.out',
                transformPerspective: 800
            });
        });

        card.addEventListener('mouseleave', () => {
            gsap.to(card, {
                rotateX: 0,
                rotateY: 0,
                duration: 0.6,
                ease: 'power2.out'
            });
        });
    });
}

/* ═══════════════════════════════════════════
   FEATURE CARDS
   ═══════════════════════════════════════════ */
function initFeatureCards() {
    const features = gsap.utils.toArray('.gsap-feature-card');
    if (!features.length) return;

    features.forEach((card, i) => {
        gsap.from(card, {
            opacity: 0,
            y: 50,
            scale: 0.9,
            duration: 0.7,
            delay: i * 0.15,
            ease: 'back.out(1.4)',
            scrollTrigger: {
                trigger: card,
                start: 'top 85%',
                toggleActions: 'play none none none'
            }
        });
    });

    // Animate icons spinning in
    gsap.utils.toArray('.gsap-feature-icon').forEach((icon, i) => {
        gsap.from(icon, {
            scale: 0,
            rotation: -180,
            duration: 0.8,
            delay: i * 0.15 + 0.3,
            ease: 'back.out(2)',
            scrollTrigger: {
                trigger: icon,
                start: 'top 85%',
                toggleActions: 'play none none none'
            }
        });
    });
}

/* ═══════════════════════════════════════════
   FLOATING ORB PARALLAX
   ═══════════════════════════════════════════ */
function initFloatingOrbs() {
    const orbs = gsap.utils.toArray('.glow-orb');
    orbs.forEach(orb => {
        gsap.to(orb, {
            y: '+=40',
            x: '+=20',
            duration: 4 + Math.random() * 3,
            repeat: -1,
            yoyo: true,
            ease: 'sine.inOut'
        });
    });
}

/* ═══════════════════════════════════════════
   SCROLL INDICATOR - Bounce + fade
   ═══════════════════════════════════════════ */
function initScrollIndicator() {
    const indicator = document.querySelector('.scroll-indicator');
    if (!indicator) return;

    // Wheel animation
    gsap.to('.scroll-indicator .wheel', {
        y: 14,
        opacity: 0,
        duration: 1,
        repeat: -1,
        ease: 'power2.in'
    });

    // Fade out on scroll
    gsap.to(indicator, {
        opacity: 0,
        scrollTrigger: {
            trigger: indicator,
            start: 'top 90%',
            end: 'top 60%',
            scrub: true
        }
    });
}

/* ═══════════════════════════════════════════
   MAGNETIC BUTTONS
   ═══════════════════════════════════════════ */
function initMagneticButtons() {
    const buttons = document.querySelectorAll('.btn-cinema-primary, .btn-cinema-outline');
    buttons.forEach(btn => {
        btn.addEventListener('mousemove', (e) => {
            const rect = btn.getBoundingClientRect();
            const x = e.clientX - rect.left - rect.width / 2;
            const y = e.clientY - rect.top - rect.height / 2;
            gsap.to(btn, {
                x: x * 0.2,
                y: y * 0.2,
                duration: 0.3,
                ease: 'power2.out'
            });
        });
        btn.addEventListener('mouseleave', () => {
            gsap.to(btn, { x: 0, y: 0, duration: 0.5, ease: 'elastic.out(1, 0.4)' });
        });
    });
}

/* ═══════════════════════════════════════════
   COUNTER ANIMATION (for stats)
   ═══════════════════════════════════════════ */
function initCounters() {
    gsap.utils.toArray('[data-counter]').forEach(el => {
        const target = parseInt(el.getAttribute('data-counter'));
        const obj = { val: 0 };
        gsap.to(obj, {
            val: target,
            duration: 2,
            ease: 'power2.out',
            scrollTrigger: {
                trigger: el,
                start: 'top 80%',
                toggleActions: 'play none none none'
            },
            onUpdate: () => {
                el.textContent = Math.round(obj.val).toLocaleString();
            }
        });
    });
}

/* ═══════════════════════════════════════════
   PAGE TRANSITION
   ═══════════════════════════════════════════ */
function initPageTransition() {
    // Smooth entrance for all pages
    gsap.from('main', {
        opacity: 0,
        y: 30,
        duration: 0.6,
        ease: 'power2.out',
        delay: 0.1
    });
}

/* ═══════════════════════════════════════════
   PARTICLE BACKGROUND (lightweight)
   ═══════════════════════════════════════════ */
function initParticles() {
    const canvas = document.getElementById('gsap-particles');
    if (!canvas) return;

    const ctx = canvas.getContext('2d');
    canvas.width = window.innerWidth;
    canvas.height = window.innerHeight;

    const particles = [];
    const count = 50;

    for (let i = 0; i < count; i++) {
        particles.push({
            x: Math.random() * canvas.width,
            y: Math.random() * canvas.height,
            radius: Math.random() * 1.5 + 0.5,
            vx: (Math.random() - 0.5) * 0.3,
            vy: (Math.random() - 0.5) * 0.3,
            alpha: Math.random() * 0.5 + 0.1
        });
    }

    function animate() {
        ctx.clearRect(0, 0, canvas.width, canvas.height);
        particles.forEach(p => {
            p.x += p.vx;
            p.y += p.vy;
            if (p.x < 0) p.x = canvas.width;
            if (p.x > canvas.width) p.x = 0;
            if (p.y < 0) p.y = canvas.height;
            if (p.y > canvas.height) p.y = 0;

            ctx.beginPath();
            ctx.arc(p.x, p.y, p.radius, 0, Math.PI * 2);
            ctx.fillStyle = `rgba(229, 9, 20, ${p.alpha})`;
            ctx.fill();
        });
        requestAnimationFrame(animate);
    }
    animate();

    window.addEventListener('resize', () => {
        canvas.width = window.innerWidth;
        canvas.height = window.innerHeight;
    });
}

// Init particles on hero pages
if (document.querySelector('.gsap-hero')) {
    initParticles();
}
