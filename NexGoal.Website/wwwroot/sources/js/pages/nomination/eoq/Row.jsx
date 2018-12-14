import React, { Component, Fragment } from 'react';
import moment from 'moment';

import { resolveClientUrl } from '../../../common-functions';
import { DateFormat } from '../../../constants';
import { getCssClassNominationStatus, nominationSettingStatuses } from '../common';

class Row extends Component {
    formatDate(value) {
        return moment(value).format(DateFormat);
    }

    handleDeleteClick = () => {
        this.props.onShowDeleteModal(this.props.eoq);
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
                        <a className="fa fa-trash-o remove" title="Delete" onClick={this.handleDeleteClick} />
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
        let eoq = this.props.eoq;
        let statusIndex = nominationSettingStatuses.findIndex(x => x.value === eoq.statusId);

        return (
            <tr>
                <td className="text-center">{this.props.rowIndex}</td>
                <td className="text-center">{eoq.quarter}</td>
                <td className="text-center">{eoq.year}</td>
                <td className="text-center">{this.formatDate(eoq.startDateForNominator)}</td>
                <td className="text-center">{this.formatDate(eoq.endDateForNominator)}</td>
                <td className="text-center">{this.formatDate(eoq.remindDateForNominator)}</td>
                <td className="text-center">{this.formatDate(eoq.endDateForCommittee)}</td>
                <td className={"text-center " + getCssClassNominationStatus(eoq.statusId)}>{nominationSettingStatuses[statusIndex].text}</td>
                <td className="text-center">{this.renderCellAction()}</td>
            </tr>
        );
    }
}

export default Row;