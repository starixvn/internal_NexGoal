import React, { Component, Fragment } from 'react';
import moment from 'moment';

import commonFunctions from '../../../common-functions';

class Row extends Component {
    constructor(props) {
        super(props);
    }

    formatDate(value) {
        return moment(value).format('DD MMM, YYYY');
    }

    getCssClassNominationStatus(statusId) {
        let className = '';

        switch (statusId) {
            case 1:
                className = 'in-progress';
                break;

            case 2:
                className = 'closed';
                break;

            default:
                className = 'not-started';
                break;
        }

        return 'nomination-period-status ' + className;
    }

    handleDeleteClick() {
        this.props.showDeleteModal(this.props.eoy);
    }

    renderCellAction() {
        const currentYear = new Date().getFullYear();
        const eoy = this.props.eoy;

        if (eoy.year >= currentYear && this.props.isHost) {
            return (
                <Fragment>
                    <a className="fa fa-pencil-square-o" title="Edit"
                        href={commonFunctions.getRootUrl() + 'EOY/Edit/' + eoy.eoqyId}
                    />
                    {eoy.statusId === 0 &&
                        <a className="fa fa-trash-o remove" title="Delete" onClick={this.handleDeleteClick.bind(this)} />
                    }
                </Fragment>
            );
        }

        return (
            <a className="fa fa-search" title="Details"
                href={commonFunctions.getRootUrl() + 'EOY/Details/' + eoy.eoqyId}
            />
        );
    }

    render() {
        const eoy = this.props.eoy;

        return (
            <tr>
                <td className="text-center">{this.props.rowIndex}</td>
                <td className="text-center">{eoy.year}</td>
                <td className="text-center">{this.formatDate(eoy.startDateForNominator)}</td>
                <td className="text-center">{this.formatDate(eoy.endDateForNominator)}</td>
                <td className="text-center">{this.formatDate(eoy.remindDateForNominator)}</td>
                <td className="text-center">{this.formatDate(eoy.endDateForCommittee)}</td>
                <td className={"text-center " + this.getCssClassNominationStatus(eoy.statusId)}>{eoy.statusToString}</td>
                <td className="text-center">{this.renderCellAction()}</td>
            </tr>
        );
    }
}

export default Row;