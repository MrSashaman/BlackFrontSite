const manifestoData = {
    "about": [
        { id: "tw-title", text: "Манифест Свободных Общин" },
        { id: "tw-subtitle", text: "Свобода человека • Самоуправление общин • Солидарность народов" },
        { id: "tw-text", text: "Этот проект исходит из одной простой и радикальной мысли: человек не рождается подданным. Ни государство, ни собственник, ни большинство не получают естественного права распоряжаться его жизнью. Свободное общество начинается там, где люди берут устройство общей жизни в собственные руки. Наша цель — порядок без хозяина, основанный на правилах, которые создаются самими участниками." }
    ],
    "principles": [
        { id: "tw-p-title", text: "Основные принципы" },
        { id: "tw-p-subtitle", text: "Идейное ядро нашего движения" },
        { id: "tw-p-text", text: "Мы строим свободное анархо-коммунистическое общество на прочном фундаменте:" }
    ],
    "structure": [
        { id: "tw-s-title", text: "Экономическое устройство" },
        { id: "tw-s-subtitle", text: "Рабочее самоуправление и общественное владение" },
        { id: "tw-s-text", text: "Земля, крупная инфраструктура и природные ресурсы находятся в распоряжении тех, кто ими пользуется — трудовых коллективов и коммун. Мы отвергаем как власть капитала над жизненной необходимостью, так и государственную опеку, заменяя их добровольной федерацией снизу вверх." }
    ]
};

const typingSpeed = 30; 
const lineDelay = 400;  

let activeSessions = {};

function createCursor() {
    const cursor = document.createElement("span");
    cursor.className = "cursor";
    cursor.innerText = "|";
    return cursor;
}

function typeSectionSequence(sectionId) {
    let session = activeSessions[sectionId];
    const sequence = manifestoData[sectionId];

    if (session.currentItemIndex >= sequence.length) {
        setTimeout(() => { 
            if (session.cursorElement) session.cursorElement.remove(); 
        }, 2000);
        return;
    }

    const currentItem = sequence[session.currentItemIndex];
    const targetElement = document.getElementById(currentItem.id);

    if (!targetElement) return;

    if (session.currentCharIndex === 0) {
        if (session.cursorElement) session.cursorElement.remove();
        session.cursorElement = createCursor();
        targetElement.appendChild(session.cursorElement);
    }

    if (session.currentCharIndex < currentItem.text.length) {
        const char = currentItem.text.charAt(session.currentCharIndex);
        session.cursorElement.insertAdjacentText("beforebegin", char);
        session.currentCharIndex++;
        setTimeout(() => typeSectionSequence(sectionId), typingSpeed);
    } else {
        session.currentItemIndex++;
        session.currentCharIndex = 0;
        setTimeout(() => typeSectionSequence(sectionId), lineDelay);
    }
}

function initScrollTracking() {
    const observerOptions = {
        root: null,
        rootMargin: "0px",
        threshold: 0.2 
    };

    const observer = new IntersectionObserver((entries, observer) => {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                const sectionId = entry.target.id;
                
                if (!activeSessions[sectionId]) {
                    activeSessions[sectionId] = {
                        currentItemIndex: 0,
                        currentCharIndex: 0,
                        cursorElement: null
                    };
                    typeSectionSequence(sectionId);
                    observer.unobserve(entry.target);
                }
            }
        });
    }, observerOptions);

    document.querySelectorAll("section").forEach(section => {
        if (manifestoData[section.id]) {
            observer.observe(section);
        }
    });
}

window.addEventListener("DOMContentLoaded", initScrollTracking);


function initSidebarHighlight() {
    const sections = document.querySelectorAll("main section");
    const navLinks = document.querySelectorAll(".sidebar-nav a");

    window.addEventListener("scroll", () => {
        let currentSectionId = "";
        
        sections.forEach(section => {
            const sectionTop = section.offsetTop;
            if (window.scrollY >= sectionTop - 200) {
                currentSectionId = section.getAttribute("id");
            }
        });

        navLinks.forEach(link => {
            link.style.color = "#64748b"; 
            if (link.getAttribute("href") === `#${currentSectionId}`) {
                link.style.color = "var(--text-color)"; 
                link.style.fontWeight = "700";
            } else {
                link.style.fontWeight = "500";
            }
        });
    });
}

window.addEventListener("DOMContentLoaded", initSidebarHighlight);
