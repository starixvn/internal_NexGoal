import React, { Component, Fragment } from 'react';
import moment from 'moment';

import { resolveClientUrl } from '../../../common-functions';

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
        this.props.showDeleteModal(this.props.eoq);
    }

    renderCellAction() {
        const currentYear = new Date().getFullYear();
        const eoq = this.props.eoq;

        if (eoq.year >= currentYear && this.props.isHost) {
            return (
                <Fragment>
                    <a className="fa fa-pencil-square-o" title="Edit"
                        href={resolveClientUrl('EOQ/Edit/' + eoq.eoqyId)}
                    />
                    {eoq.statusId === 0 &&
                        <a className="fa fa-trash-o remove" title="Delete" onClick={this.handleDeleteClick.bind(this)} />
                    }
                </Fragment>
            );
        }

        return (
            <a className="fa fa-search" title="Details"
                href={resolveClientUrl('EOQ/Details/' + eoq.eoqyId)}
            />
        );
    }

    render() {
        const eoq = this.props.eoq;

        return (
            <tr>
                <td className="text-center">{this.props.rowIndex}</td>
                <td>{eoq.quarterToString}</td>
                <td className="text-center">{eoq.year}</td>
                <td className="text-center">{this.formatDate(eoq.startDateForNominator)}</td>
                <td className="text-center">{this.formatDate(eoq.endDateForNominator)}</td>
                <td className="text-center">{this.formatDate(eoq.remindDateForNominator)}</td>
                <td className="text-center">{this.formatDate(eoq.endDateForCommittee)}</td>
                <td className={"text-center " + this.getCssClassNominationStatus(eoq.statusId)}>{eoq.statusToString}</td>
                <td className="text-center">{this.renderCellAction()}</td>
            </tr>
        );
    }
}

export default Row;