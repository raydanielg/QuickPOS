    function printDivs() {
        var printContents = document.getElementById('printable-form').outerHTML;
    var printWindow = window.open('', '_blank', 'width=600,height=600');

    printWindow.document.open();
    printWindow.document.write('<html><head><title>Print</title>');
        printWindow.document.write(
        '<link rel="stylesheet" href="/assets/css/bootstrap.css"><link rel="stylesheet" href="/assets/css/style.css"><link rel="stylesheet" href="/app.css"><html>'
            );
            printWindow.document.write('</head><body>');
                printWindow.document.write(printContents);
                printWindow.document.write('</body></html>');
        printWindow.document.close();
    setTimeout(() => {
            printWindow.print();
    }, 1000);
  }
