import React, { Component, Fragment } from 'react';
import getResource from '../../../../resourceHelper';
import Rows from './Rows';
import EditMemberPA from '../EditPAForm';

class Table extends Component {
    constructor(props) {
        super(props);

        this.state = {
            isShowEditModal: false,
            paRecordId: null
        };
    }

    shouldComponentUpdate(nextProps, nextState) {
        if (JSON.stringify(nextProps.data) !== JSON.stringify(this.props.data)
            || JSON.stringify(nextProps.gradeOptions) !== JSON.stringify(this.props.gradeOptions)
            || nextState.isShowEditModal !== this.state.isShowEditModal
            || nextState.paRecordId !== this.state.paRecordId) {
            return true;
        }

        return false;
    }

    onClickEditPA = (paRecordId) => {
        this.setState({
            isShowEditModal: true,
            paRecordId: paRecordId
        });
    }

    onCloseModal = () => {
        this.setState({
            isShowEditModal: false,
            paRecordId: null
        });
    }

    onEditSucess = () => {
        this.onCloseModal();
        this.props.renderPA();
    }

    renderRow = () => {
        const isNoData = this.props.data.length === 0;
        if (isNoData) {
            return (
                <tr>
                    <td colSpan={7}>There is no information</td>
                </tr>
            );
        } else {
            return (
                this.props.data.map(group => {
                    return (
                        <Rows key={group.groupName}
                            data={group}
                            colSpan={7}
                            gradeOptions={this.props.gradeOptions}
                            onClickEditPA={this.onClickEditPA}
                        />
                    );
                }));
        }
    };

    render() {
        return (
            <Fragment>
                <table className="table table-bordered" >
                    <thead>
                        <tr>
                            <th>{getResource('NickName')}</th>
                            <th>Toeic</th>
                            <th>{getResource('Self')}</th>
                            <th>{getResource('ProjectManager')}</th>
                            <th>{getResource('Evaluator')}</th>
                            <th>{getResource('Reviewer')}</th>
                            <th>{getResource('Status')}</th>
                        </tr>
                    </thead>
                    <tbody>
                        {
                            this.renderRow()
                        }
                    </tbody>
                </table>
                {
                    this.state.isShowEditModal &&
                    <EditMemberPA
                        isShowModal={this.state.isShowEditModal}
                        paRecordId={this.state.paRecordId}
                        gradeOptions={this.props.gradeOptions}
                        onCloseModal={this.onCloseModal}
                        onSubmitSucess={this.onEditSucess}
                    />
                }
            </Fragment>
        );
    }
}

export default Table;