import React, { Component } from 'react';
import { Button, Modal } from 'react-bootstrap';
import axios from 'axios';
import moment from 'moment';

import { axiosError, resolveClientUrl } from '../../../common-functions';
import getResource from '../../../resourceHelper';
import { DateFormat } from '../../../constants';

export default class Discipline extends Component {
    constructor(props) {
        super(props);
        this.state = {
            disciplines: []
        };
    }

    componentDidMount() {
        axios.get(resolveClientUrl('Common/GetDisciplineByEmployeeId'), {
            params: {
                memberId: this.props.memberId ? this.props.memberId : null
            }
        }).then(response =>
            this.setState({
                disciplines: response.data
            })).catch(axiosError);
    }

    renderDisciplines = (disciplines) => {
        if (disciplines.length > 0) {
            return (
                disciplines.map((discipline, index) => {
                    return (
                        <tr>
                            <td className="text-center">{index + 1}</td>
                            <td>{discipline.description}</td>
                            <td className="text-center">{moment(discipline.effectiveDate).format(DateFormat)}</td>
                        </tr>
                    );
                })
            );
        }

        return (
            <tr>
                <td colSpan="3" className="text-center">{getResource('NoRecordFound')}</td>
            </tr>
        );
    }

    render() {
        const disciplines = this.state.disciplines;
        return (
            <Modal keyboard backdrop="static"
                show={this.props.isShowModal} onHide={this.props.onCloseModal}
                dialogClassName="custom-modal modal-md form-performance edit "
            >
                <Modal.Header>
                    <Modal.Title>{getResource('Discipline')}</Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    <table className="table table-bordered">
                        <thead>
                            <tr>
                                <th>#</th>
                                <th>{getResource('Description')}</th>
                                <th>{getResource('IssueDate')}</th>
                            </tr>
                        </thead>
                        <tbody>
                            {
                                this.renderDisciplines(disciplines)
                            }
                        </tbody>
                    </table>
                </Modal.Body>
                <Modal.Footer>
                    <Button className="btn-cancel" onClick={this.props.onCloseModal}>{getResource('Close')}</Button>
                </Modal.Footer>
            </Modal>
        );
    }
}