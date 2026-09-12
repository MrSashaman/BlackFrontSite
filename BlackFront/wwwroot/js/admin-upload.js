
let newsEditor = null;

document.addEventListener("DOMContentLoaded", () => {
    const editorElement = document.querySelector('#NewPost_Content');
    
    if (editorElement) {
        const { ClassicEditor, Essentials, Paragraph, Heading, Bold, Italic, Link, List, BlockQuote } = window.CKEditor5;

        if (!ClassicEditor) {
            console.error("Критическая ошибка: ClassicEditor не найден в сборке ckeditor.js");
            return;
        }

        ClassicEditor
            .create(editorElement, {
                plugins: [ Essentials, Paragraph, Heading, Bold, Italic, Link, List, BlockQuote ],
                toolbar: [ 'heading', '|', 'bold', 'italic', 'link', 'bulletedList', 'numberedList', 'blockQuote', 'undo', 'redo' ]
            })
            .then(editor => {
                newsEditor = editor;
                console.log("CKEditor 5 успешно запущен локально!");
            })
            .catch(error => {
                console.error("Ошибка при инициализации CKEditor:", error);
            });
    }
});

async function uploadFileAutomatically(input) {
    const files = input.files;
    const statusSpan = document.getElementById('upload-status');
    
    if (!files || files.length === 0) return;
    const file = files[0];

    const allowedExtensions = ['txt', 'json', 'pdf', 'docx'];
    const fileExtension = file.name.split('.').pop().toLowerCase();
    
    if (!allowedExtensions.includes(fileExtension)) {
        statusSpan.style.color = 'red';
        statusSpan.innerText = 'Ошибка: Неподдерживаемый формат файла.';
        input.value = ''; 
        return;
    }

    statusSpan.style.color = 'orange';
    statusSpan.innerText = 'Загрузка и обработка структуры документа...';

    const formData = new FormData();
    formData.append("file", file);

    try {
        const tokenElement = document.querySelector('input[name="__RequestVerificationToken"]');
        const token = tokenElement ? tokenElement.value : "";
        
        const response = await fetch('?handler=ParseFile', {
            method: 'POST',
            headers: {
                'RequestVerificationToken': token
            },
            body: formData
        });

        if (response.ok) {
            const data = await response.json();
            if (data.success) {
                if (newsEditor) {
                    newsEditor.setData(data.content);
                } else {
                    const textarea = document.getElementById('NewPost_Content');
                    if (textarea) textarea.value = data.content;
                }
                
                statusSpan.style.color = 'green';
                statusSpan.innerText = 'Текст с сохранением оформления успешно вставлен!';
            } else {
                statusSpan.style.color = 'red';
                statusSpan.innerText = 'Ошибка: ' + data.error;
            }
        } else {
            statusSpan.style.color = 'red';
            statusSpan.innerText = `Ошибка сервера: ${response.status}`;
        }
    } catch (error) {
        statusSpan.style.color = 'red';
        statusSpan.innerText = 'Не удалось отправить файл (ошибка сети).';
        console.error('Детали сетевой ошибки:', error);
    }
}
