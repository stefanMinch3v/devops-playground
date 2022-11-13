import { CompanyModel } from './company.model';
import { SelectedCompanyModel } from './selected-company.model';

export class CompanyWrapper {
    constructor(
        public companies: Array<CompanyModel>,
        public selectedData: SelectedCompanyModel) {}
}
