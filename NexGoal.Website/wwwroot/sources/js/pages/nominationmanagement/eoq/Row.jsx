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

    renderCellAction() {
        let currentYear = new Date().getFullYear();

        if (this.props.eoq.year >= currentYear && this.props.isHost) {
            return (
                <Fragment>
                    <a className="fa fa-pencil-square-o" title="Edit"
                        href={commonFunctions.getRootUrl() + 'EOQ/Edit/' + this.props.eoq.eoqyId}></a>
                    {this.props.eoq.statusId === 0 &&
                        <a className="fa fa-trash-o remove" title="Delete"></a>
                    }
                </Fragment>
            );
        }

        return (
            <a className="fa fa-search" title="Details"
                href={commonFunctions.getRootUrl() + 'EOQ/Details/' + this.props.eoq.eoqyId}></a>
        );
    }

    render() {
        return (
            <tr>
                <td className="text-center">{this.props.rowIndex}</td>
                <td>{this.props.eoq.quarterToString}</td>
                <td className="text-center">{this.props.eoq.year}</td>
                <td className="text-center">{this.formatDate(this.props.eoq.startDateForNominator)}</td>
                <td className="text-center">{this.formatDate(this.props.eoq.endDateForNominator)}</td>
                <td className="text-center">{this.formatDate(this.props.eoq.remindDateForNominator)}</td>
                <td className="text-center">{this.formatDate(this.props.eoq.endDateForCommittee)}</td>
                <td className={"text-center " + this.getCssClassNominationStatus(this.props.eoq.statusId)}>{this.props.eoq.statusToString}</td>
                <td className="text-center">{this.renderCellAction()}</td>
            </tr>
        );
    }
}

export default Row;