﻿const modal = document.getElementById("appointmentModal");
const newAppointmentBtn = document.getElementById("newAppointmentBtn");
const closeModalBtn = document.getElementById("closeModal");
const appointmentForm = document.getElementById("appointmentForm");
const appointmentTable = document.getElementById("appointmentTable").getElementsByTagName('tbody')[0];
let editingRow = null;

// Função para abrir o modal
newAppointmentBtn.addEventListener("click", function () {
    window.location.href = 'AbrirAdicionarAgendamento'
});

// Função para fechar o modal
closeModalBtn.addEventListener("click", function () {
    modal.style.display = "none";
});

// Função para fechar o modal clicando fora dele
window.onclick = function (event) {
    if (event.target === modal) {
        modal.style.display = "none";
    }
};

// Função para adicionar ou editar agendamento
appointmentForm.addEventListener("submit", function (e) {
    e.preventDefault();

    const nomeCliente = document.getElementById("nomeCliente").value;
    const placaCarro = document.getElementById("placaCarro").value;
    const data = document.getElementById("data").value;
    const servico = document.getElementById("servico").value;
    const plano = document.getElementById("plano").value;
    const preco = document.getElementById("preco").value;

    const precoNumero = parseFloat(preco.replace('MZ', '').replace(',', '.'));

    const dataAtualizada = {
        nomeCliente: nomeCliente,
        placaCarro: placaCarro,
        data: formatDateToCustomFormat(data),
        plano: plano,
        servico: servico,
        preco: precoNumero
    };

    console.log("data: " + dataAtualizada.data);
    const url = editingRow ? "/Dashboard/EditarAgendamento" : "/Dashboard/AdicionarAgendamento";

    fetch(url, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/x-www-form-urlencoded',
        },
        body: new URLSearchParams(dataAtualizada)
    })
        .then(response => {
            if (response.ok) {
                if (editingRow) {
                    const updatedRow = editingRow;
                    updatedRow.cells[0].textContent = nomeCliente;
                    updatedRow.cells[1].textContent = plano;
                    updatedRow.cells[2].textContent = servico;
                    updatedRow.cells[3].textContent = placaCarro;
                    updatedRow.cells[4].textContent = data;
                    updatedRow.cells[5].textContent = `${precoNumero.toFixed(2)} MZ`;
                } else {
                    location.reload(); 
                }
            } else {
                alert('Erro ao salvar agendamento!');
            }
        })
        .catch(error => console.error('Erro:', error));

    modal.style.display = "none";
});

// Função para editar agendamentos
function editAppointment(button) {
    const editingRow = button.parentElement.parentElement;
    const id = editingRow.getAttribute('data-id');

    // Redireciona para a página de edição, passando o ID na URL
    window.location.href = '/Dashboard/AbrirEditarAgendamento?idAgendamento=' + id;
}

// Função para deletar agendamentos
function deleteAppointment(button) {
    if (confirm("Tem certeza que deseja cancelar o agendamento?")) {
        const row = button.parentElement.parentElement;
        const id = row.dataset.id;

        fetch('/Dashboard/DeletarAgendamento', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded',
            },
            body: new URLSearchParams({ id: id })
        })
            .then(response => response.ok ? row.remove() : alert('Erro ao deletar agendamento!'))
            .catch(error => console.error('Erro:', error));
    }
}

// Atualizar preço ao mudar serviço ou plano
function updatePreco() {
    const tipoServico = document.getElementById("servico").value;
    const plano = document.getElementById("plano").value;

    if (tipoServico && plano) {
        fetch(`/Dashboard/ObterPreco?tipoServico=${tipoServico}&plano=${plano}`)
            .then(response => response.json())
            .then(data => {
                if (data.preco) {
                    const preco = parseFloat(data.preco);
                    if (!isNaN(preco)) {
                        document.getElementById("preco").value = `${preco.toFixed(2)} MZ`;
                    } else {
                        console.error('Preço inválido recebido:', data.preco);
                        document.getElementById("preco").value = "Preço inválido";
                    }
                } else {
                    console.error('Preço não disponível.');
                    document.getElementById("preco").value = "Preço não disponível";
                }
            })
            .catch(error => console.error('Erro ao buscar o preço:', error));
    }
}

document.getElementById("servico").addEventListener("change", updatePreco);
document.getElementById("plano").addEventListener("change", updatePreco);

function formatDateToCustomFormat(dateString) {
    const date = new Date(dateString);

    const year = date.getFullYear(); // Ano completo
    const month = (date.getMonth() + 1).toString().padStart(2, '0'); // Mês com dois dígitos
    const day = date.getDate().toString().padStart(2, '0'); // Dia com dois dígitos

    // Retorna a data no formato "yyyy/MM/dd"
    return `${year}/${month}/${day}`;
}


// Função para abrir o modal de edição de produto e preencher os dados
function editProduct(button) {
    const editingRow = button.parentElement.parentElement;
    const id = editingRow.getAttribute('data-id');

    // Redireciona para a página de edição, passando o ID na URL
    window.location.href = '/Dashboard/AbrirEditarProduto?id=' + id;
}
