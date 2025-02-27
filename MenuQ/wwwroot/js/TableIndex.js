document.addEventListener("DOMContentLoaded", function () {
    const areaSelect = document.getElementById("area-input");
    const tableInput = document.getElementById("table-input");
    const textTop1 = document.getElementById("textTop1");

    function updateTextTop1() {
        const areaName = areaSelect.options[areaSelect.selectedIndex].text;
        const firstTable = document.querySelector('.table-checkbox:checked') ? document.querySelector('.table-checkbox:checked').value : "";

        if (areaName && firstTable) {
            textTop1.innerText = `${areaName} - Bàn ${firstTable}`;
        } else {
            textTop1.innerText = areaName ? `${areaName} - Chọn bàn` : "";
        }
    }

    function refreshTableList() {
        const selectedAreaId = areaSelect.value;
        document.querySelectorAll('.area-section').forEach(section => {
            if (section.getAttribute('data-area-id') === selectedAreaId) {
                section.style.display = 'block';
            } else {
                section.style.display = 'none';
            }
        });
        // Clear the selected tables and input value
        document.querySelectorAll('.table-checkbox').forEach(checkbox => {
            checkbox.checked = false;
        });
        document.getElementById("TableInput").value = "";
    }

    areaSelect.addEventListener("change", function () {
        updateTextTop1();
        updateMaxTableNumber();
        refreshTableList(); // Làm mới danh sách các bàn khi khu vực thay đổi
    });

    tableInput.addEventListener("input", updateTextTop1);

    // Kích hoạt sự kiện thay đổi để hiển thị bàn của khu vực đầu tiên khi tải trang
    function initialize() {
        const areaSelect = document.getElementById("area-input");
        if (areaSelect.options.length > 0) {
            areaSelect.selectedIndex = 0;
            updateTextTop1();
            updateMaxTableNumber();
            refreshTableList(); // Làm mới danh sách các bàn cho khu vực đầu tiên
            areaSelect.dispatchEvent(new Event('change'));
        }
    }

    window.onload = initialize;
});

document.getElementById("confirmTables").addEventListener("click", function () {
    let selectedTables = [];
    document.querySelectorAll('.table-checkbox:checked').forEach(checkbox => {
        selectedTables.push(checkbox.value);
    });

    let tableText = selectedTables.length > 0 ? selectedTables.join(", ") : "-";
    document.getElementById("TableInput").value = tableText;
    document.getElementById("SelectedTable").innerText = tableText;

    let area = document.getElementById("area-input").options[document.getElementById("area-input").selectedIndex].text;
    document.getElementById("SelectedArea").innerText = area;

    // Hiển thị mã QR chỉ cho bàn đầu tiên
    if (selectedTables.length > 0) {
        let firstTable = selectedTables[0]; // Chỉ lấy bàn đầu tiên
        let qrData = encodeURIComponent(`Khu vực: ${area} - Bàn: ${firstTable}`);
        document.getElementById("QRImage").src = `https://api.qrserver.com/v1/create-qr-code/?size=150x150&data=${qrData}`;

        // Cập nhật textTop1 với khu vực và bàn đầu tiên
        document.getElementById("textTop1").innerText = `${area} - Bàn ${firstTable}`;
    } else {
        document.getElementById("QRImage").src = "";
        document.getElementById("textTop1").innerText = "";
    }

    $('#tableModal').modal('hide');
});

function validateForm() {
    let isValid = true;
    const tableInput = document.getElementById("table-input");
    const tableError = document.getElementById("table-error");

    const table = parseInt(tableInput.value, 10);
    const maxTableNumber = parseInt(tableInput.max, 10);

    if (table < 1 || table > maxTableNumber || isNaN(table)) {
        tableError.style.display = "inline";
        tableError.textContent = `Please enter a valid table number (1-${maxTableNumber}).`;
        isValid = false;
        clearQRCode()
    } else {
        tableError.style.display = "none";
        generateQRCodeWithCustomOptions()
        tableInput.setAttribute("max", maxTableNumber);
    }
    return isValid;
}

function updateMaxTableNumber() {
    var areaSelect = document.getElementById("area-input");
    var selectedOption = areaSelect.options[areaSelect.selectedIndex];
    var maxTableNumber = selectedOption.getAttribute("data-max");
    const tableInput = document.getElementById("table-input");

    tableInput.setAttribute("max", maxTableNumber);
    tableInput.value = "";
    document.getElementById("table-error").style.display = "none";
}

function generateQRCodeWithCustomOptions() {
    const color = document.getElementById("color-input").value;
    const backgroundColor = document.getElementById("background-color-input").value;
    const topText = document.getElementById("top-text").value;
    const bottomText = document.getElementById("bottom-text").value;

    document.getElementById("textTop").innerText = topText;
    document.getElementById("textBottom").innerText = bottomText;

    const baseURL = "https://sample.com";
    if (baseURL.trim() !== "") {
        const urlWithParams = `Thank For Use MenuQ`;

        generateQRCode("qrcode1", urlWithParams, color, backgroundColor);
    } else {
        alert("Please enter a base URL to generate a QR code.");
    }
}

function generateQRCode(canvasId, text, color, backgroundColor) {
    const canvas = document.getElementById(canvasId);
    const ctx = canvas.getContext("2d");
    ctx.clearRect(0, 0, canvas.width, canvas.height);

    const qrSize = 200;
    canvas.width = qrSize;
    canvas.height = qrSize;

    QRCode.toCanvas(canvas, text, {
        color: {
            dark: color,
            light: backgroundColor
        },
        width: qrSize,
    }, function (error) {
        if (error) console.error(error);
        else {
            document.getElementById("printButton").style.display = "block";
        }
    });
}

document.getElementById("editTableBtn").addEventListener("click", function () {
    $('#tableModal').modal('show');
});

function clearQRCode() {
    const canvas = document.getElementById("qrcode1");
    const ctx = canvas.getContext("2d");
    ctx.clearRect(0, 0, canvas.width, canvas.height);
    document.getElementById("textTop").innerText = "";
    document.getElementById("textBottom").innerText = "";
    document.getElementById("printButton").style.display = "none";
}

function redirectToPrintQR() {
    const areaId = document.getElementById("area-input").value;
    const tableNumber = document.getElementById("table-input").value;
    const topText = encodeURIComponent(document.getElementById("top-text").value);
    const bottomText = encodeURIComponent(document.getElementById("bottom-text").value);
    const color = encodeURIComponent(document.getElementById("color-input").value);
    const backgroundColor = encodeURIComponent(document.getElementById("background-color-input").value);

    // Lấy tất cả tên bàn đã chọn
    let selectedTables = document.getElementById("TableInput").value;

    if (!areaId || !tableNumber) {
        alert("Vui lòng chọn khu vực và nhập số bàn.");
        return;
    }

    window.location.href = `/Admin/Table/PrintQR?areaId=${areaId}&tableCount=${tableNumber}&topText=${topText}&bottomText=${bottomText}&color=${color}&backgroundColor=${backgroundColor}&selectedTables=${encodeURIComponent(selectedTables)}`;
}