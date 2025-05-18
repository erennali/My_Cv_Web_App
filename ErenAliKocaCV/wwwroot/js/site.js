// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

document.addEventListener('DOMContentLoaded', function() {
    // Base title for the website
    const baseTitle = 'Eren Ali Koca | Software Developer';
    
    // Menü butonu ve collapse elementlerini alıyoruz
    const menuToggle = document.querySelector('.navbar-toggler');
    const menuCollapse = document.getElementById('ftco-nav');
    
    // iOS ve dokunmatik cihazlarda scroll'u düzeltmek için zorunlu ayarlamalar
    document.documentElement.style.height = 'auto';
    document.body.style.height = 'auto';
    document.documentElement.style.overflowY = 'visible';
    document.body.style.overflowY = 'visible';
    
    // iOS Safari'de kaydırmayı sağlamak için
    window.scrollTo(0, 1);
    
    // Overlay'ı temizlemek için yardımcı fonksiyon
    function removeMenuOverlay() {
        const overlay = document.getElementById('menu-overlay');
        if (overlay) {
            overlay.style.opacity = '0';
            setTimeout(() => {
                if (overlay && overlay.parentNode) {
                    overlay.parentNode.removeChild(overlay);
                }
            }, 300);
        }
    }
    
    // Menüyü kapatmak için yardımcı fonksiyon
    function closeMenu() {
        if (menuCollapse && menuCollapse.classList.contains('show')) {
            menuCollapse.classList.remove('show');
            if (menuToggle) {
                menuToggle.classList.remove('active');
            }
            
            // Menü kapandığında sayfa kaydırmayı etkinleştir
            document.body.style.overflow = 'auto';
            
            // Overlay'ı temizle
            removeMenuOverlay();
            
            // Sayfa kaydırmayı hafif bir gecikme ile etkinleştir
            setTimeout(function() {
                document.body.style.overflow = 'auto';
            }, 300);
        }
    }
    
    // Menü butonuna dokunmayı geliştirmek için
    if (menuToggle) {
        // Dokunma kapsama alanını artırmak için ek div ekle
        const touchArea = document.createElement('div');
        touchArea.style.position = 'fixed';
        touchArea.style.top = '0';
        touchArea.style.right = '0';
        touchArea.style.width = '60px';
        touchArea.style.height = '60px';
        touchArea.style.zIndex = '1999';
        touchArea.style.background = 'transparent';
        document.body.appendChild(touchArea);
        
        // Geniş dokunma alanı için event listener
        touchArea.addEventListener('click', function(e) {
            e.preventDefault();
            e.stopPropagation();
            menuToggle.click();
        });
        
        // Navbar-toggler'ı görünür yapmak için CSS ekleme
        menuToggle.style.pointerEvents = 'auto';
        menuToggle.style.cursor = 'pointer';
    }
    
    // Mobil cihazlarda menü açıldığında kaydırılabilirliği optimize etme
    function optimizeMenuScroll() {
        if (menuCollapse && menuCollapse.classList.contains('show')) {
            // Menü açıkken ekranın kaydırılmasını engelleme
            document.body.style.overflow = 'hidden';
            
            // Menünün kaydırılabilir olması için
            menuCollapse.style.overflowY = 'auto';
            menuCollapse.style.maxHeight = '85vh';
            menuCollapse.style.display = 'block';
            
            // Tüm menü öğelerinin görünür olmasını sağla
            const navItems = menuCollapse.querySelectorAll('.nav-item');
            navItems.forEach((item, index) => {
                item.style.display = 'block';
                item.style.margin = '15px 0';
                item.style.opacity = '1';
                
                // Son öğeye fazladan alt boşluk ekle
                if (index === navItems.length - 1) {
                    item.style.marginBottom = '30px';
                }
            });
            
            // Menu içeriğini ortala ve tam genişlik ver
            const navLinks = menuCollapse.querySelectorAll('.nav-link');
            navLinks.forEach(link => {
                link.style.textAlign = 'center';
                link.style.padding = '12px 15px';
                link.style.margin = '0 auto';
                link.style.display = 'inline-block';
            });
            
            // Sayfa pozisyonunu hatırla (menü kapatıldığında dönmek için)
            window.menuScrollPosition = window.scrollY;
        } else {
            // Menü kapalıyken sayfa kaydırmayı etkinleştir
            document.body.style.overflow = 'auto';

            // Eğer önceki scroll pozisyonu kaydedildiyse ve menü GERÇEKTEN YENİ KAPANDIysa sayfayı oraya geri getir.
            // Bu, resize gibi olaylarda sayfanın gereksiz yere kaymasını engeller.
            // Bu satırı ŞİMDİLİK YORUM SATIRI YAPARAK test edelim.
            // if (window.menuScrollPosition !== undefined && !menuCollapse.classList.contains('show')) {
            //     // window.scrollTo(0, window.menuScrollPosition);
            // }
        }
    }
    
    // Debounce function to limit the rate at which updatePageTitle function executes
    function debounce(func, wait) {
        let timeout;
        return function() {
            const context = this;
            const args = arguments;
            clearTimeout(timeout);
            timeout = setTimeout(() => func.apply(context, args), wait);
        };
    }
    
    // Function to update the page title based on visible section
    function updatePageTitle() {
        const sections = document.querySelectorAll('section[id]');
        const pageTitles = document.querySelectorAll('.page-title');
        const scrollPosition = window.scrollY + 200; // Add offset to detect section earlier
        
        let activeSectionFound = false;
        
        // First try to find by section IDs
        for (let i = 0; i < sections.length; i++) {
            const section = sections[i];
            const top = section.offsetTop;
            const height = section.offsetHeight;
            
            if (scrollPosition >= top && scrollPosition <= top + height) {
                // Find the page title in this section
                const pageTitle = section.querySelector('.page-title');
                if (pageTitle) {
                    document.title = `${pageTitle.textContent.trim()} | Eren Ali Koca`;
                    activeSectionFound = true;
                    break;
                }
            }
        }
        
        // If no section was found active, default to the base title
        if (!activeSectionFound) {
            document.title = baseTitle;
        }
    }
    
    // Lazy load images
    function lazyLoadImages() {
        const lazyImages = document.querySelectorAll('[data-src]');
        if ('IntersectionObserver' in window) {
            const imageObserver = new IntersectionObserver((entries, observer) => {
                entries.forEach(entry => {
                    if (entry.isIntersecting) {
                        const img = entry.target;
                        img.src = img.dataset.src;
                        img.removeAttribute('data-src');
                        imageObserver.unobserve(img);
                    }
                });
            });
            lazyImages.forEach(img => imageObserver.observe(img));
        } else {
            // Fallback for browsers without IntersectionObserver support
            lazyImages.forEach(img => {
                img.src = img.dataset.src;
                img.removeAttribute('data-src');
            });
        }
    }
    
    // Menüyü açıp kapatma fonksiyonu - güçlendirilmiş versiyon
    if (menuToggle) {
        menuToggle.addEventListener('click', function(e) {
            console.log('Menu toggle clicked'); // Debug için
            e.preventDefault();
            e.stopPropagation();
            
            // Toggle menu state
            menuToggle.classList.toggle('active');
            
            if (menuCollapse) {
                // Menü durumunu değiştir
                menuCollapse.classList.toggle('show');
                
                // Menü görünürlüğünü optimize et
                optimizeMenuScroll();
                
                // Menü açıkken sayfa arka planını karartan overlay ekle
                if (menuCollapse.classList.contains('show')) {
                    // Overlay ekle
                    let overlay = document.getElementById('menu-overlay');
                    if (!overlay) {
                        overlay = document.createElement('div');
                        overlay.id = 'menu-overlay';
                        overlay.style.position = 'fixed';
                        overlay.style.top = '0';
                        overlay.style.left = '0';
                        overlay.style.width = '100%';
                        overlay.style.height = '100%';
                        overlay.style.backgroundColor = 'rgba(0,0,0,0.7)';
                        overlay.style.zIndex = '1040';
                        overlay.style.opacity = '0';
                        overlay.style.transition = 'opacity 0.3s ease';
                        document.body.appendChild(overlay);
                        
                        // Overlay'a tıklanınca menüyü kapat
                        overlay.addEventListener('click', function() {
                            closeMenu();
                        });
                    }
                    
                    // Overlay'ı göster
                    setTimeout(() => {
                        overlay.style.opacity = '1';
                    }, 10);
                    
                    document.body.style.overflow = 'hidden';
                } else {
                    // Overlay'ı kaldır
                    removeMenuOverlay();
                    document.body.style.overflow = 'auto';
                }
            }
        }, { capture: true });
    }
    
    // Menü linklerine tıklandığında menüyü kapat ve overlay'ı temizle
    const menuLinks = document.querySelectorAll('.navbar-nav .nav-link');
    menuLinks.forEach(link => {
        link.addEventListener('click', function() {
            closeMenu();
        });
    });
    
    // Menüyü elle yeniden hesapla - Observer ile
    const resizeObserver = new ResizeObserver(debounce(() => {
        if (menuCollapse && menuCollapse.classList.contains('show')) {
            optimizeMenuScroll();
        }
    }, 100));
    
    if (menuCollapse) {
        resizeObserver.observe(menuCollapse);
    }
    
    // Listen for scroll events with debounce to improve performance
    window.addEventListener('scroll', debounce(updatePageTitle, 200));
    
    // Scroll event'lerini daha yumuşak hale getirme
    let ticking = false;
    window.addEventListener('scroll', function() {
        if (!ticking) {
            window.requestAnimationFrame(function() {
                ticking = false;
            });
            ticking = true;
        }
    });
    
    // İlk yüklemede sayfanın scrollable olduğundan emin ol
    // setTimeout(function() {
    //     window.scrollTo(0, 0);
    // }, 100);
    
    // Initial update
    setTimeout(updatePageTitle, 100);
    
    // Initialize lazy loading
    if (document.readyState === 'complete') {
        lazyLoadImages();
    } else {
        window.addEventListener('load', lazyLoadImages);
    }
    
    // Add smooth scrolling to all internal links
    document.querySelectorAll('a[href^="#"]').forEach(anchor => {
        anchor.addEventListener('click', function(e) {
            e.preventDefault();
            const targetId = this.getAttribute('href');
            if (targetId === '#') return;
            
            const targetElement = document.querySelector(targetId);
            if (targetElement) {
                // Menüyü kapat
                closeMenu();
                
                // Kısa bir gecikme ile scroll işlemini yap
                setTimeout(function() {
                    // Safari için smooth scroll olmadığı için düz scrollIntoView kullan
                    const isSafari = /^((?!chrome|android).)*safari/i.test(navigator.userAgent);
                    const isMobile = /iPhone|iPad|iPod|Android/i.test(navigator.userAgent);
                    
                    if (isSafari || isMobile) {
                        // Safari ve mobil cihazlarda smooth scroll bazen çalışmıyor
                        const yOffset = -70; // header yüksekliği kadar offset
                        const y = targetElement.getBoundingClientRect().top + window.pageYOffset + yOffset;
                        window.scrollTo({top: y, behavior: 'auto'});
                    } else {
                        // Diğer tarayıcılarda smooth scroll
                        targetElement.scrollIntoView({
                            behavior: 'smooth',
                            block: 'start'
                        });
                    }
                }, 100);
            }
        });
    });
    
    // Touch olaylarını düzeltme (mobilin başa dönme sorunu)
    let touchStartY = 0;
    document.addEventListener('touchstart', function(e) {
        touchStartY = e.touches[0].clientY;
    }, { passive: true });
    
    document.addEventListener('touchmove', function(e) {
        const touchY = e.touches[0].clientY;
        const diff = touchStartY - touchY;
        
        // Çok küçük dokunuşların sayfayı başa sarmasını engelle
        if (Math.abs(diff) < 10) {
            e.preventDefault();
        }
        
        // Menü açıksa ve touchmove olayı navbar dışındaysa, menüyü kapat
        if (menuCollapse && menuCollapse.classList.contains('show') && 
            !e.target.closest('.navbar-collapse') && 
            !e.target.closest('.navbar-toggler')) {
            closeMenu();
        }
    }, { passive: false });
    
    // Dokunma olayları için fazladan tıklama desteği
    document.addEventListener('touchend', function(e) {
        if (menuToggle && (e.target === menuToggle || menuToggle.contains(e.target))) {
            console.log('Touchend on menuToggle'); // Debug için
            e.preventDefault();
            e.stopPropagation();
            
            // Yapay tıklama olayı oluştur
            const clickEvent = new MouseEvent('click', {
                bubbles: true,
                cancelable: true,
                view: window
            });
            menuToggle.dispatchEvent(clickEvent);
        }
    }, { passive: false });
    
    // Sayfa yükleme tamamlandığında, herhangi bir overlay kalmışsa temizle
    window.addEventListener('load', function() {
        removeMenuOverlay();
        document.body.style.overflow = 'auto';
    });
    
    // Sayfa yüklendiğinde ve yeniden boyutlandırıldığında menü optimizasyonu
    window.addEventListener('load', optimizeMenuScroll);
    window.addEventListener('resize', debounce(optimizeMenuScroll, 100));
    
    // Overlay'ı kaldırmak için olay dinleyicisi ekle
    document.addEventListener('click', function(e) {
        // Menü ve buton dışında herhangi bir yere tıklanırsa
        if (menuCollapse && 
            menuCollapse.classList.contains('show') && 
            !e.target.closest('.navbar-collapse') && 
            !e.target.closest('.navbar-toggler')) {
            closeMenu();
        }
    });
});
