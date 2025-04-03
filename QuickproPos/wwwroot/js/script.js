	function printDiv() {
		var divContents = document.getElementById('printable-form');

		var opt = {
			margin: 2,
			filename: 'Print.pdf',
			image: { type: 'jpeg', quality: 0.98 },
			html2canvas: { scale: 5 },
			//jsPDF: { unit: 'in', format: 'letter', orientation: 'portrait' }
		};
		html2pdf().set(opt).from(divContents).save();
	}
	function printDivWithName(_filename) {
		var divContents = document.getElementById('PDFinvoice');

		var opt = {
			margin: 4,
			filename: _filename + '.pdf',
			image: { type: 'jpeg', quality: 0.98 },
			html2canvas: { scale: 5 },
			jsPDF: {
				orientation: 'p',
				unit: 'mm',
				format: 'a4',
				putOnlyUsedFonts: true,
				floatPrecision: 16 // or "smart", default is 16
			}
			//pagebreak: { before: '#myID', after: ['#after1', '#after2'], avoid: 'img' }
			//pagebreak: { mode: ['avoid-all', 'css', 'legacy'] }
		};
		html2pdf().set(opt).from(divContents).save();
	}

	window.PrintPdf = () => {
		var divContents = document.getElementById('PDFinvoice');




		var opt = {
			margin: 2,
			filename: 'PurchaseInvoice.pdf',
			image: { type: 'jpeg', quality: 0.98 },
			html2canvas: { scale: 5 },
			//jsPDF: { unit: 'in', format: 'letter', orientation: 'portrait' }
		};

		// New Promise-based usage:
		html2pdf().set(opt).from(divContents).save();

		//html2pdf(divContents, opt);
	};
// Select 2
if ($('.select').length > 0) {
	$('.select').select2({
		minimumResultsForSearch: -1,
		width: '100%'
	});
}
	function exportToExcel(_filename) {
		var location = 'data:application/vnd.ms-excel;base64,';
		var excelTemplate = '<html> ' +
			'<head> ' +
			'<meta http-equiv="content-type" content="text/plain; charset=UTF-8"/> ' +
			'</head> ' +
			'<body> ' +
			document.getElementById(_filename).innerHTML +
			'</body> ' +
			'</html>'
		window.location.href = location + window.btoa(excelTemplate);
}



function renderSalesChart(months, totals) {
	const ctx = document.getElementById('salesChart').getContext('2d');
	const parsedMonths = JSON.parse(months);
	const parsedTotals = JSON.parse(totals);

	new Chart(ctx, {
		type: 'line',
		data: {
			labels: parsedMonths,
			datasets: [
				{
					label: 'Sales',
					data: parsedTotals,
					borderColor: 'rgba(75, 192, 192, 1)',
					backgroundColor: 'rgba(75, 192, 192, 0.2)',
					borderWidth: 2,
				},
			],
		},
		options: {
			responsive: true,
			plugins: {
				legend: {
					display: true,
				},
			},
			scales: {
				x: {
					title: {
						display: true,
						text: 'Months',
					},
				},
				y: {
					title: {
						display: true,
						text: 'Grand Total',
					},
				},
			},
		},
	});
}

