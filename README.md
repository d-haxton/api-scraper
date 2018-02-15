api-scraper

Prereq:

Run in elevated command prompt
netsh http add urlacl url="http://+:8888/nancy/" user="Everyone"
netsh http add urlacl url="http://127.0.0.1:8898/nancy/" user="Everyone"
netsh http add urlacl url="http://+:8889/nancytoo/" user="Everyone"
